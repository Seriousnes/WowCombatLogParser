using System;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

public interface ICombatLogSegmentProvider
{
    ICombatLogFileContext? Open(string filePath, Func<string, CombatLogEvent?> parseLine);

    IReadOnlyList<CombatLogEvent> LoadSegment(Segment segment);

    ValueTask<IReadOnlyList<CombatLogEvent>> LoadSegmentAsync(Segment segment, CancellationToken cancellationToken = default);
}
