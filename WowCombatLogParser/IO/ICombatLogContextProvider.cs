using System;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

public unsafe interface ICombatLogContextProvider : IDisposable
{
    IReadOnlyList<Segment> Open(string filePath, ICombatLogParser parser);

    long FileLength { get; }

    bool TryGetSpan(long startOffset, int length, out ReadOnlySpan<byte> span);

    bool TryGetPointer(long startOffset, int length, out byte* pointer);

    ValueTask ReadExactlyAsync(long startOffset, Memory<byte> destination, CancellationToken cancellationToken = default);
}
