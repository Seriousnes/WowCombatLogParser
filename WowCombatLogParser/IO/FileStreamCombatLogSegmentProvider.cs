using StackExchange.Profiling;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

public sealed class FileStreamCombatLogSegmentProvider : ICombatLogSegmentProvider
{
    private readonly Dictionary<string, string> segmentTypes;
    private readonly Lock gate = new();
    private Dictionary<Segment, IReadOnlyList<CombatLogEvent>> segmentCache = [];
    private Dictionary<Segment, Task<IReadOnlyList<CombatLogEvent>>> segmentTaskCache = [];

    public FileStreamCombatLogSegmentProvider()
    {
        var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
        segmentTypes = assemblyTypes
            .Where(x => x.Namespace == nameof(WoWCombatLogParser))
            .Where(x => x.IsAssignableTo(typeof(IFightStart)))
            .ToDictionary(k => k.GetCustomAttribute<DiscriminatorAttribute>()!.Value, v =>
            {
                var startAttributeValue = v.GetCustomAttribute<DiscriminatorAttribute>()!.Value;
                var endAttributeValue = $"{startAttributeValue[..startAttributeValue.LastIndexOf('_')]}_END";

                return assemblyTypes
                    .SingleOrDefault(x => x.GetCustomAttribute<DiscriminatorAttribute>() is DiscriminatorAttribute discriminator && discriminator.Value == endAttributeValue)
                    ?.GetCustomAttribute<DiscriminatorAttribute>()
                    ?.Value
                    ?? string.Empty;
            });
    }

    public ICombatLogFileContext? Open(string filePath, Func<string, CombatLogEvent?> parseLine)
    {
        using (gate.EnterScope())
        {
            segmentCache = [];
            segmentTaskCache = [];
        }

        var segments = new List<Segment>();
        var context = new RandomAccessCombatLogFileContext(filePath, segments, parseLine);

        using var stream = new FileStream(filePath, new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.ReadWrite,
            BufferSize = StreamExtensions.GetBufferSize(filePath),
            Options = FileOptions.RandomAccess,
        });

        using var outerStep = MiniProfiler.Current.Step("Get Segments");

        long i = -1;
        while ((i = stream.IndexOfAny(segmentTypes.Keys, i + 1, IndexMode.LineStart, out var match)) >= 0)
        {
            var endToken = segmentTypes[match.Value!];
            var endPosition = stream.IndexOf(endToken, i, IndexMode.LineEnd);
            if (endPosition >= 0)
            {
                using var innerStep = MiniProfiler.Current.Step("Preparse start/end");

                var startLine = stream.GetCurrentLine(ref i, IndexMode.LineStart);
                var endLine = stream.GetCurrentLine(ref endPosition, IndexMode.LineEnd);

                var length = checked((int)(endPosition - i));
                var segment = new Segment(context, i, length)
                {
                    Start = startLine is { } ? parseLine(startLine) as IFightStart : null,
                    End = endLine is { } ? parseLine(endLine) as IFightEnd : null,
                };

                segments.Add(segment);
            }

            i = stream.IndexOf(Environment.NewLine, endPosition, IndexMode.LineEnd);
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
}
