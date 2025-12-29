using System;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

public interface ICombatLogFileContext : IDisposable
{
    string FilePath { get; }
    long FileLength { get; }

    IReadOnlyList<Segment> Segments { get; }

    bool TryGetSpan(long start, int length, out ReadOnlySpan<byte> span);

    ValueTask ReadExactlyAsync(long start, Memory<byte> destination, CancellationToken cancellationToken = default);

    IReadOnlyList<CombatLogEvent> LoadEvents(Segment segment);
    ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(Segment segment, CancellationToken cancellationToken = default);

    IReadOnlyList<CombatLogEvent> LoadEvents(long startOffset, int length);
    ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(long startOffset, int length, CancellationToken cancellationToken = default);
}
