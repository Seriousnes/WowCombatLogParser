using System;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

public abstract class CombatLogFileContextBase(string filePath, long fileLength, IReadOnlyList<Segment> segments, Func<string, CombatLogEvent?> parseLine) : ICombatLogFileContext
{
    public string FilePath { get; } = filePath;
    public long FileLength { get; } = fileLength;

    public IReadOnlyList<Segment> Segments { get; } = segments;

    protected Func<string, CombatLogEvent?> ParseLine { get; } = parseLine;

    public abstract bool TryGetSpan(long start, int length, out ReadOnlySpan<byte> span);

    public abstract ValueTask ReadExactlyAsync(long start, Memory<byte> destination, CancellationToken cancellationToken = default);

    public virtual IReadOnlyList<CombatLogEvent> LoadEvents(Segment segment) => LoadEvents(segment.StartOffset, segment.Length);

    public virtual ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(Segment segment, CancellationToken cancellationToken = default)
        => LoadEventsAsync(segment.StartOffset, segment.Length, cancellationToken);

    public abstract IReadOnlyList<CombatLogEvent> LoadEvents(long startOffset, int length);

    public abstract ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(long startOffset, int length, CancellationToken cancellationToken = default);

    public abstract void Dispose();
}
