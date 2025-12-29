using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Profiling;

namespace WoWCombatLogParser.IO;

public sealed class MemoryMappedCombatLogSegmentProvider : ICombatLogSegmentProvider
{
    private readonly SegmentToken[] tokens;
    private readonly int maxStartTokenLength;
    private readonly SearchValues<byte> startTokenFirstBytes;

    private readonly Lock gate = new();
    private Dictionary<Segment, IReadOnlyList<CombatLogEvent>> segmentCache = [];
    private Dictionary<Segment, Task<IReadOnlyList<CombatLogEvent>>> segmentTaskCache = [];

    private readonly struct SegmentToken(string start, string end)
    {
        public readonly byte[] StartBytes = Encoding.UTF8.GetBytes(start);
        public readonly byte[] EndBytes = Encoding.UTF8.GetBytes(end);
    }

    public MemoryMappedCombatLogSegmentProvider()
    {
        var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

        var startTypes = assemblyTypes
            .Where(x => x.Namespace == nameof(WoWCombatLogParser))
            .Where(x => x.IsAssignableTo(typeof(IFightStart)))
            .ToArray();

        var tokenList = new List<SegmentToken>(startTypes.Length);
        foreach (var startType in startTypes)
        {
            var startValue = startType.GetCustomAttribute<DiscriminatorAttribute>()?.Value;
            if (string.IsNullOrWhiteSpace(startValue))
            {
                continue;
            }

            var endValue = $"{startValue[..startValue.LastIndexOf('_')]}_END";
            var endType = assemblyTypes.SingleOrDefault(x => x.GetCustomAttribute<DiscriminatorAttribute>() is DiscriminatorAttribute discriminator && discriminator.Value == endValue);
            var endToken = endType?.GetCustomAttribute<DiscriminatorAttribute>()?.Value;
            if (string.IsNullOrWhiteSpace(endToken))
            {
                continue;
            }

            tokenList.Add(new SegmentToken(startValue, endToken));
        }

        tokens = [.. tokenList];
        maxStartTokenLength = tokens.Length > 0 ? tokens.Max(t => t.StartBytes.Length) : 0;

        if (tokens.Length == 0)
        {
            startTokenFirstBytes = SearchValues.Create([(byte)'\n']);
            return;
        }

        var firstBytes = tokens.Select(t => t.StartBytes[0]).Distinct().ToArray();
        startTokenFirstBytes = SearchValues.Create(firstBytes);
    }

    public ICombatLogFileContext? Open(string filePath, Func<string, CombatLogEvent?> parseLine)
    {
        if (tokens.Length == 0)
        {
            return null;
        }

        using (gate.EnterScope())
        {
            segmentCache = [];
            segmentTaskCache = [];
        }

        var fileInfo = new FileInfo(filePath);
        if (fileInfo is not { Length: > 0 } )
        {
            return null;
        }

        var segments = new List<Segment>();
        var context = new MemoryMappedCombatLogFileContext(filePath, fileInfo.Length, segments, parseLine);

        using var step = MiniProfiler.Current.Step("Get Segments (MMF)");

        unsafe
        {
            byte* ptr = context.Pointer;

            long cursor = 0;
            while (cursor < fileInfo.Length)
            {
                long remaining = fileInfo.Length - cursor;
                int windowSize = (int)Math.Min(remaining, 1024 * 1024 * 1024);

                var window = new ReadOnlySpan<byte>(ptr + cursor, windowSize);
                int index = window.IndexOfAny(startTokenFirstBytes);

                if (index == -1)
                {
                    if (remaining <= windowSize)
                    {
                        break;
                    }

                    cursor += windowSize - (maxStartTokenLength - 1);
                    continue;
                }

                long absolutePos = cursor + index;

                if (absolutePos < 2 || *(ptr + absolutePos - 1) != (byte)' ' || *(ptr + absolutePos - 2) != (byte)' ')
                {
                    cursor = absolutePos + 1;
                    continue;
                }

                int matchedTokenIndex = -1;
                for (int t = 0; t < tokens.Length; t++)
                {
                    var startBytes = tokens[t].StartBytes;
                    if (absolutePos + startBytes.Length > fileInfo.Length)
                    {
                        continue;
                    }

                    var potential = new ReadOnlySpan<byte>(ptr + absolutePos, startBytes.Length);
                    if (potential.SequenceEqual(startBytes))
                    {
                        matchedTokenIndex = t;
                        break;
                    }
                }

                if (matchedTokenIndex < 0)
                {
                    cursor = absolutePos + 1;
                    continue;
                }

                var token = tokens[matchedTokenIndex];
                long startLineStart = FindLineStart(ptr, fileInfo.Length, absolutePos);
                long startLineEnd = FindLineEnd(ptr, fileInfo.Length, startLineStart);

                long searchStart = absolutePos + token.StartBytes.Length;
                long endTokenPos = FindToken(ptr, fileInfo.Length, searchStart, token.EndBytes);
                if (endTokenPos == -1)
                {
                    cursor = absolutePos + 1;
                    continue;
                }

                long endLineStart = FindLineStart(ptr, fileInfo.Length, endTokenPos);
                long endLineEnd = FindLineEnd(ptr, fileInfo.Length, endLineStart);

                long segmentStart = startLineEnd;
                long segmentEnd = endLineStart;
                if (segmentEnd > segmentStart)
                {
                    var segmentLength = checked((int)(segmentEnd - segmentStart));
                    var segment = new Segment(context, segmentStart, segmentLength)
                    {
                        Start = parseLine(DecodeLine(ptr, startLineStart, startLineEnd)) as IFightStart,
                        End = parseLine(DecodeLine(ptr, endLineStart, endLineEnd)) as IFightEnd,
                    };
                    segments.Add(segment);
                }

                cursor = endLineEnd;
            }
        }

        return context;
    }

    public IReadOnlyList<CombatLogEvent> LoadSegment(Segment segment)
    {
        Task<IReadOnlyList<CombatLogEvent>>? taskToWait = null;

        using (gate.EnterScope())
        {
            if (segmentCache.TryGetValue(segment, out var cached))
            {
                return cached;
            }

            if (segmentTaskCache.TryGetValue(segment, out var task))
            {
                taskToWait = task;
            }
            else
            {
                var parsed = segment.Context.LoadEvents(segment);
                segmentCache.Add(segment, parsed);
                return parsed;
            }
        }

        taskToWait!.Wait();
        var completed = taskToWait.Result;

        using (gate.EnterScope())
        {
            segmentCache[segment] = completed;
            segmentTaskCache.Remove(segment);
        }

        return completed;
    }

    public ValueTask<IReadOnlyList<CombatLogEvent>> LoadSegmentAsync(Segment segment, CancellationToken cancellationToken = default)
    {
        using (gate.EnterScope())
        {
            if (segmentCache.TryGetValue(segment, out var cached))
            {
                return ValueTask.FromResult(cached);
            }

            if (segmentTaskCache.TryGetValue(segment, out var existing))
            {
                if (existing.IsCompletedSuccessfully)
                {
                    var result = existing.Result;
                    segmentCache[segment] = result;
                    segmentTaskCache.Remove(segment);
                    return ValueTask.FromResult(result);
                }

                return new ValueTask<IReadOnlyList<CombatLogEvent>>(existing);
            }

            var task = segment.Context.LoadEventsAsync(segment, cancellationToken).AsTask();
            segmentTaskCache.Add(segment, task);
            return new ValueTask<IReadOnlyList<CombatLogEvent>>(task);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe long FindLineStart(byte* basePtr, long fileLength, long position)
    {
        long current = position - 1;
        while (current >= 0)
        {
            if (*(basePtr + current) == (byte)'\n')
            {
                return current + 1;
            }
            current--;
        }
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe long FindLineEnd(byte* basePtr, long fileLength, long position)
    {
        long current = position;
        while (current < fileLength)
        {
            if (*(basePtr + current) == (byte)'\n')
            {
                return current + 1;
            }
            current++;
        }
        return fileLength;
    }

    private unsafe long FindToken(byte* basePtr, long fileLength, long startOffset, byte[] token)
    {
        if (token.Length == 0)
        {
            return -1;
        }

        long cursor = startOffset;
        var tokenSpan = new ReadOnlySpan<byte>(token);

        while (cursor < fileLength)
        {
            long remaining = fileLength - cursor;
            int windowSize = (int)Math.Min(remaining, 1024 * 1024 * 1024);
            var window = new ReadOnlySpan<byte>(basePtr + cursor, windowSize);

            int index = window.IndexOf(tokenSpan);
            if (index == -1)
            {
                if (remaining <= windowSize)
                {
                    break;
                }

                cursor += windowSize - (token.Length - 1);
                continue;
            }

            return cursor + index;
        }

        return -1;
    }

    private static unsafe string DecodeLine(byte* basePtr, long start, long endExclusive)
    {
        var length = (int)(endExclusive - start);
        if (length <= 0)
        {
            return string.Empty;
        }

        var span = new ReadOnlySpan<byte>(basePtr + start, length);
        while (!span.IsEmpty && (span[^1] == (byte)'\n' || span[^1] == (byte)'\r'))
        {
            span = span[..^1];
        }
        return Encoding.UTF8.GetString(span);
    }
}
