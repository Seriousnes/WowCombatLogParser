using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace WoWCombatLogParser.IO;

public sealed class FileStreamCombatLogContextProvider : ICombatLogContextProvider
{
    private readonly Dictionary<string, string> segmentTypes;
    private SafeFileHandle? handle;

    public long FileLength { get; private set; }

    public FileStreamCombatLogContextProvider()
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

    public IReadOnlyList<Segment> Open(string filePath, ICombatLogParser parser)
    {
        handle?.Dispose();
        handle = File.OpenHandle(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, FileOptions.RandomAccess);

        var fileInfo = new FileInfo(filePath);
        FileLength = fileInfo.Length;

        var segments = new List<Segment>();

        using var stream = new FileStream(filePath, new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.ReadWrite,
            BufferSize = StreamExtensions.GetBufferSize(filePath),
            Options = FileOptions.RandomAccess,
        });

        long i = -1;
        while ((i = stream.IndexOfAny(segmentTypes.Keys, i + 1, IndexMode.LineStart, out var match)) >= 0)
        {
            var endToken = segmentTypes[match.Value!];
            var endPosition = stream.IndexOf(endToken, i, IndexMode.LineEnd);
            if (endPosition >= 0)
            {
                var startLine = stream.GetCurrentLine(ref i, IndexMode.LineStart);
                var endLine = stream.GetCurrentLine(ref endPosition, IndexMode.LineEnd);

                var length = checked((int)(endPosition - i));
                var segment = new Segment(i, length)
                {
                    Start = startLine is { } ? parser.GetCombatLogEvent(startLine) as IFightStart : null,
                    End = endLine is { } ? parser.GetCombatLogEvent(endLine) as IFightEnd : null,
                };

                segments.Add(segment);
            }

            i = stream.IndexOf(Environment.NewLine, endPosition, IndexMode.LineEnd);
        }

        return segments;
    }

    public bool TryGetSpan(long startOffset, int length, out ReadOnlySpan<byte> span)
    {
        span = default;
        return false;
    }

    public unsafe bool TryGetPointer(long startOffset, int length, out byte* pointer)
    {
        pointer = null;
        return false;
    }

    public async ValueTask ReadExactlyAsync(long startOffset, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        if (handle is null)
        {
            throw new InvalidOperationException("Provider has not been opened.");
        }

        long offset = startOffset;
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

    public void Dispose()
    {
        handle?.Dispose();
        handle = null;
        FileLength = 0;
    }
}
