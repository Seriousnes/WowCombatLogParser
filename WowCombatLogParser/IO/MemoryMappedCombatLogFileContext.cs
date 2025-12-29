using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

public sealed class MemoryMappedCombatLogFileContext : CombatLogFileContextBase
{
    private readonly MemoryMappedFile mmf;
    private readonly MemoryMappedViewAccessor accessor;

    private unsafe readonly byte* basePtr;
    private readonly long pointerOffset;

    internal unsafe byte* Pointer => basePtr + pointerOffset;

    public unsafe MemoryMappedCombatLogFileContext(string filePath, long fileLength, IReadOnlyList<Segment> segments, Func<string, CombatLogEvent?> parseLine)
        : base(filePath, fileLength, segments, parseLine)
    {
        mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        byte* p = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref p);
        basePtr = p;
        pointerOffset = accessor.PointerOffset;
    }

    public override unsafe bool TryGetSpan(long start, int length, out ReadOnlySpan<byte> span)
    {
        span = default;

        if (start < 0 || length < 0) return false;
        if (start + length > FileLength) return false;

        span = new ReadOnlySpan<byte>(basePtr + pointerOffset + start, length);
        return true;
    }

    public override ValueTask ReadExactlyAsync(long start, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        if (!TryGetSpan(start, destination.Length, out var span))
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        span.CopyTo(destination.Span);
        return ValueTask.CompletedTask;
    }

    public override unsafe IReadOnlyList<CombatLogEvent> LoadEvents(long startOffset, int length)
    {
        if (!TryGetSpan(startOffset, length, out var span))
        {
            throw new ArgumentOutOfRangeException(nameof(startOffset));
        }

        return CombatLogEventParsing.ParseSequential(span, ParseLine);
    }

    public override unsafe ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(long startOffset, int length, CancellationToken cancellationToken = default)
    {
        if (startOffset < 0 || length < 0) throw new ArgumentOutOfRangeException(nameof(startOffset));
        if (startOffset + length > FileLength) throw new ArgumentOutOfRangeException(nameof(startOffset));

        var segmentPtr = Pointer + startOffset;
        return ValueTask.FromResult(CombatLogEventParsing.ParseParallelFromPointer(segmentPtr, length, ParseLine));
    }

    public override void Dispose()
    {
        accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        accessor.Dispose();
        mmf.Dispose();
    }
}
