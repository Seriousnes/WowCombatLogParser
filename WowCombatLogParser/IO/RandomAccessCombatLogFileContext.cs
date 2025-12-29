using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace WoWCombatLogParser.IO;

public sealed class RandomAccessCombatLogFileContext(string filePath, IReadOnlyList<Segment> segments, Func<string, CombatLogEvent?> parseLine) : CombatLogFileContextBase(filePath, new FileInfo(filePath).Length, segments, parseLine)
{
    private readonly SafeFileHandle handle = File.OpenHandle(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, FileOptions.RandomAccess);

    public override bool TryGetSpan(long start, int length, out ReadOnlySpan<byte> span)
    {
        span = default;
        return false;
    }

    public override async ValueTask ReadExactlyAsync(long start, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        long offset = start;
        int remaining = destination.Length;
        while (remaining > 0)
        {
            int bytesRead = await RandomAccess.ReadAsync(handle, destination[^remaining..], offset, cancellationToken).ConfigureAwait(false);
            if (bytesRead == 0)
            {
                throw new EndOfStreamException($"Unexpected EOF while reading {remaining} bytes at offset {offset}.");
            }

            remaining -= bytesRead;
            offset += bytesRead;
        }
    }

    public override IReadOnlyList<CombatLogEvent> LoadEvents(long startOffset, int length)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(length);
        var memory = owner.Memory[..length];

        long offset = startOffset;
        int remaining = memory.Length;
        while (remaining > 0)
        {
            int bytesRead = RandomAccess.Read(handle, memory.Span[(memory.Length - remaining)..], offset);
            if (bytesRead == 0)
            {
                throw new EndOfStreamException($"Unexpected EOF while reading {remaining} bytes at offset {offset}.");
            }

            remaining -= bytesRead;
            offset += bytesRead;
        }

        return CombatLogEventParsing.ParseSequential(memory.Span, ParseLine);
    }

    public override async ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(long startOffset, int length, CancellationToken cancellationToken = default)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(length);
        var memory = owner.Memory[..length];

        await ReadExactlyAsync(startOffset, memory, cancellationToken).ConfigureAwait(false);
        return CombatLogEventParsing.ParseParallelFromMemory(memory, ParseLine);
    }

    public override void Dispose() => handle.Dispose();
}
