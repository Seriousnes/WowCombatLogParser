using System;
using System.Buffers;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

public sealed class MemoryMappedCombatLogContextProvider : ICombatLogContextProvider
{
    private readonly SegmentToken[] tokens;
    private readonly int maxStartTokenLength;
    private readonly SearchValues<byte> startTokenFirstBytes;

    private MemoryMappedFile? mmf;
    private MemoryMappedViewAccessor? accessor;

    private unsafe byte* basePtr;
    private long pointerOffset;

    public long FileLength { get; private set; }

    private readonly struct SegmentToken(string start, string end)
    {
        public readonly byte[] StartBytes = Encoding.UTF8.GetBytes(start);
        public readonly byte[] EndBytes = Encoding.UTF8.GetBytes(end);
    }

    public MemoryMappedCombatLogContextProvider()
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

    public IReadOnlyList<Segment> Open(string filePath, ICombatLogParser parser)
    {
        Dispose();

        var fileInfo = new FileInfo(filePath);
        FileLength = fileInfo.Length;

        if (fileInfo is not { Length: > 0 })
        {
            return [];
        }

        mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        unsafe
        {
            byte* p = null;
            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref p);
            basePtr = p;
            pointerOffset = accessor.PointerOffset;
        }

        var segments = new List<Segment>();

        if (tokens.Length == 0)
        {
            return segments;
        }

        unsafe
        {
            byte* ptr = basePtr + pointerOffset;

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
                    var segment = new Segment(segmentStart, segmentLength)
                    {
                        Start = parser.GetCombatLogEvent(DecodeLine(ptr, startLineStart, startLineEnd)) as IFightStart,
                        End = parser.GetCombatLogEvent(DecodeLine(ptr, endLineStart, endLineEnd)) as IFightEnd,
                    };
                    segments.Add(segment);
                }

                cursor = endLineEnd;
            }
        }

        return segments;
    }

    public bool TryGetSpan(long startOffset, int length, out ReadOnlySpan<byte> span)
    {
        span = default;

        if (accessor is null)
        {
            return false;
        }

        if (startOffset < 0 || length < 0)
        {
            return false;
        }

        if (startOffset + length > FileLength)
        {
            return false;
        }

        unsafe
        {
            span = new ReadOnlySpan<byte>(basePtr + pointerOffset + startOffset, length);
            return true;
        }
    }

    public unsafe bool TryGetPointer(long startOffset, int length, out byte* pointer)
    {
        pointer = null;

        if (accessor is null)
        {
            return false;
        }

        if (startOffset < 0 || length < 0)
        {
            return false;
        }

        if (startOffset + length > FileLength)
        {
            return false;
        }

        pointer = basePtr + pointerOffset + startOffset;
        return true;
    }

    public ValueTask ReadExactlyAsync(long startOffset, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!TryGetSpan(startOffset, destination.Length, out var span))
        {
            throw new ArgumentOutOfRangeException(nameof(startOffset));
        }

        span.CopyTo(destination.Span);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        if (accessor is not null)
        {
            unsafe
            {
                accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }

            accessor.Dispose();
            accessor = null;
        }

        mmf?.Dispose();
        mmf = null;

        unsafe
        {
            basePtr = null;
        }

        pointerOffset = 0;
        FileLength = 0;
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
