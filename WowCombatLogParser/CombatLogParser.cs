using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WoWCombatLogParser.IO;

namespace WoWCombatLogParser;

/// <summary>
/// Parses World of Warcraft combat log lines and files into strongly-typed <see cref="CombatLogEvent"/> instances.
/// </summary>
/// <remarks>
/// Typical usage:
/// <list type="number">
/// <item><description>Create a parser with an <see cref="ICombatLogEventMapper"/> and an <see cref="ICombatLogContextProvider"/>.</description></item>
/// <item><description>Call <see cref="SetFilePath"/> to initialize combat log version and file context.</description></item>
/// <item><description>Call <see cref="GetSegments"/> then parse segments via <see cref="ParseSegment"/>/<see cref="ParseSegmentAsync"/>.</description></item>
/// </list>
/// Thread-safety: a single instance is safe to use for concurrent segment parsing after <see cref="SetFilePath"/> completes,
/// but calling <see cref="SetFilePath"/> concurrently with any parsing operation is not supported.
/// </remarks>
public interface ICombatLogParser
{
    /// <summary>
    /// Parses a single combat log line into a <see cref="CombatLogEvent"/>.
    /// </summary>
    /// <param name="text">A single raw combat log line.</param>
    /// <returns>The parsed event, or <see langword="null"/> if parsing failed.</returns>
    CombatLogEvent? GetCombatLogEvent(string text);

    /// <summary>
    /// Parses a single combat log line into a specific <see cref="CombatLogEvent"/> type.
    /// </summary>
    /// <typeparam name="T">Expected event type.</typeparam>
    /// <param name="line">A single raw combat log line.</param>
    /// <returns>The parsed event, or <see langword="null"/> if parsing failed.</returns>
    T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent;

    /// <summary>
    /// Asynchronously parses a single combat log line.
    /// </summary>
    /// <param name="line">A single raw combat log line.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task producing the parsed event, or <see langword="null"/> if parsing failed.</returns>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is cancelled.</exception>
    ValueTask<CombatLogEvent?> GetCombatLogEventAsync(string line, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the combat log file to parse and initializes version- and file-backed parsing state.
    /// </summary>
    /// <param name="filePath">Path to a WoW combat log file. The first line must be a <c>COMBAT_LOG_VERSION</c> header.</param>
    /// <exception cref="FileNotFoundException">Thrown if <paramref name="filePath"/> does not exist.</exception>
    /// <exception cref="InvalidDataException">Thrown if the file is empty or the first line is not a valid version header.</exception>
    void SetFilePath(string filePath);

    /// <summary>
    /// Returns the list of segments detected in the currently configured combat log file.
    /// </summary>
    /// <returns>A list of segments.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetFilePath"/> has not been called.</exception>
    IReadOnlyList<Segment> GetSegments();

    /// <summary>
    /// Asynchronously returns the list of segments detected in the currently configured combat log file.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task producing a list of segments.</returns>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is cancelled.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetFilePath"/> has not been called.</exception>
    ValueTask<IReadOnlyList<Segment>> GetSegmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses all events within the provided segment and stores the results on <see cref="Segment.Events"/>.
    /// </summary>
    /// <param name="segment">A segment obtained from <see cref="GetSegments"/>.</param>
    void ParseSegment(Segment segment);

    /// <summary>
    /// Asynchronously parses all events within the provided segment and stores the results on <see cref="Segment.Events"/>.
    /// </summary>
    /// <param name="segment">A segment obtained from <see cref="GetSegments"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is cancelled.</exception>
    ValueTask ParseSegmentAsync(Segment segment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads and parses events for an arbitrary byte range within the current combat log file.
    /// </summary>
    /// <param name="startOffset">Byte offset in the file.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <returns>A list of parsed events.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetFilePath"/> has not been called.</exception>
    IReadOnlyList<CombatLogEvent> LoadEvents(long startOffset, int length);

    /// <summary>
    /// Asynchronously loads and parses events for an arbitrary byte range within the current combat log file.
    /// </summary>
    /// <param name="startOffset">Byte offset in the file.</param>
    /// <param name="length">Number of bytes to read.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task producing a list of parsed events.</returns>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is cancelled.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetFilePath"/> has not been called.</exception>
    ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(long startOffset, int length, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets parse errors captured while parsing lines.
    /// </summary>
    /// <remarks>
    /// The key is the combat log event type discriminator string. Values contain the raw line and the exception.
    /// The dictionary is written to under a lock; however, consumers should not mutate it concurrently with parsing.
    /// </remarks>
    Dictionary<string, List<ParserError>> Errors { get; }
}

/// <summary>
/// Default implementation of <see cref="ICombatLogParser"/>.
/// </summary>
/// <remarks>
/// This type maintains file-backed state; call <see cref="SetFilePath"/> before calling APIs that require a file context.
/// Thread-safety: do not call <see cref="SetFilePath"/> concurrently with parsing operations.
/// </remarks>
public sealed class CombatLogParser : ICombatLogParser, IDisposable
{
    private readonly ICombatLogEventMapper mapper;
    private readonly ICombatLogContextProvider contextProvider;
    private string? filePath;
    private IReadOnlyList<Segment> segments = [];
    private long fileLength;
    private ConcurrentDictionary<string, ConcurrentBag<ParserError>> errors = new();

    /// <summary>
    /// Creates a new parser.
    /// </summary>
    /// <param name="mapper">Maps raw combat log lines to concrete event types.</param>
    /// <param name="contextProvider">Provides segmentation and segment loading for a combat log file.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="contextProvider"/> is <see langword="null"/>.</exception>
    public CombatLogParser(ICombatLogEventMapper mapper, ICombatLogContextProvider contextProvider)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));

        this.mapper.SetCombatLogVersion(Constants.DefaultCombatLogVersion);
    }

    /// <inheritdoc />
    public Dictionary<string, List<ParserError>> Errors => errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());

    /// <inheritdoc />
    public CombatLogEvent? GetCombatLogEvent(string line) => GetCombatLogEvent<CombatLogEvent>(line);

    /// <inheritdoc />
    public T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent
    {
        try
        {
            return mapper.GetCombatLogEvent<T>(line);
        }
        catch (Exception exception)
        {
            var eventType = exception is CombatLogParserException parserException ? parserException.EventType : CombatLogFieldReader.ReadFields(line).EventType;            
            if (!errors.TryGetValue(eventType, out var err))
            {
                err = [];
                errors.TryAdd(eventType, err);
            }
            err.Add(new ParserError(exception, line));

            return null;
        }
    }

    /// <inheritdoc />
    public ValueTask<CombatLogEvent?> GetCombatLogEventAsync(string line, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(GetCombatLogEvent(line));
    }

    /// <inheritdoc />
    public void SetFilePath(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        errors.Clear();
        this.filePath = filePath;

        segments = [];
        fileLength = 0;

        using var stream = new FileStream(filePath, new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.ReadWrite,
            BufferSize = StreamExtensions.GetBufferSize(filePath),
            Options = FileOptions.SequentialScan,
        });
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: false);
        var firstLine = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(firstLine))
            throw new InvalidDataException("File is empty or missing COMBAT_LOG_VERSION header.");

        try
        {
            var versionEvent = new CombatLogVersionEvent(firstLine);
            mapper.SetCombatLogVersion(versionEvent.Version);
        }
        catch (Exception exception)
        {
            throw new InvalidDataException("First line is not a valid COMBAT_LOG_VERSION header.", exception);
        }

        segments = contextProvider.Open(filePath, this);
        fileLength = contextProvider.FileLength;

        if (segments.Count == 0)
        {
            if (fileLength > int.MaxValue)
            {
                throw new InvalidOperationException($"File is too large to represent as a single segment: {fileLength} bytes.");
            }

            segments = [new Segment(0, (int)fileLength)];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Segment> GetSegments()
    {
        if (filePath is null)
            throw new InvalidOperationException("SetFilePath must be called before GetSegments.");

        return segments;
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<Segment>> GetSegmentsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(GetSegments());
    }

    /// <inheritdoc />
    public void ParseSegment(Segment segment)
    {
        segment.Events = LoadEvents(segment.StartOffset, segment.Length);
    }

    /// <inheritdoc />
    public async ValueTask ParseSegmentAsync(Segment segment, CancellationToken cancellationToken = default)
    {
        segment.Events = await LoadEventsAsync(segment.StartOffset, segment.Length, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IReadOnlyList<CombatLogEvent> LoadEvents(long startOffset, int length)
    {
        if (filePath is null)
            throw new InvalidOperationException("SetFilePath must be called before LoadEvents.");

        if (startOffset < 0 || length < 0)
            throw new ArgumentOutOfRangeException(nameof(startOffset));

        if (startOffset + length > fileLength)
            throw new ArgumentOutOfRangeException(nameof(startOffset));

        if (contextProvider.TryGetSpan(startOffset, length, out var span))
        {
            return ParseSequential(span);
        }

        using var owner = MemoryPool<byte>.Shared.Rent(length);
        var memory = owner.Memory[..length];
        contextProvider.ReadExactlyAsync(startOffset, memory).AsTask().GetAwaiter().GetResult();
        return ParseSequential(memory.Span);
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(long startOffset, int length, CancellationToken cancellationToken = default)
    {
        if (filePath is null)
            throw new InvalidOperationException("SetFilePath must be called before LoadEventsAsync.");

        if (startOffset < 0 || length < 0)
            throw new ArgumentOutOfRangeException(nameof(startOffset));

        if (startOffset + length > fileLength)
            throw new ArgumentOutOfRangeException(nameof(startOffset));

        cancellationToken.ThrowIfCancellationRequested();

        unsafe
        {
            if (contextProvider.TryGetPointer(startOffset, length, out var pointer))
            {
                return ValueTask.FromResult(ParseParallelFromPointer(pointer, length));
            }
        }

        return new ValueTask<IReadOnlyList<CombatLogEvent>>(LoadEventsAsyncFromRead(startOffset, length, cancellationToken));
    }

    private async Task<IReadOnlyList<CombatLogEvent>> LoadEventsAsyncFromRead(long startOffset, int length, CancellationToken cancellationToken)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(length);
        var memory = owner.Memory[..length];

        await contextProvider.ReadExactlyAsync(startOffset, memory, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return ParseParallelFromMemory(memory);
    }

    internal IReadOnlyList<CombatLogEvent> ParseSequential(ReadOnlySpan<byte> data)
    {
        var events = new List<CombatLogEvent>(capacity: 1024);
        int i = 0;

        while (i < data.Length)
        {
            int lineBreak = data[i..].IndexOf((byte)'\n');
            int lineEndExclusive = lineBreak >= 0 ? i + lineBreak : data.Length;

            var line = data[i..lineEndExclusive];
            if (line.Length > 0 && line[^1] == (byte)'\r')
            {
                line = line[..^1];
            }

            if (!line.IsEmpty)
            {
                var lineText = Encoding.UTF8.GetString(line);
                if (GetCombatLogEvent(lineText) is { } combatLogEvent)
                {
                    events.Add(combatLogEvent);
                }
            }

            if (lineBreak < 0)
            {
                break;
            }

            i = lineEndExclusive + 1;
        }

        return events;
    }

    internal IReadOnlyList<CombatLogEvent> ParseParallelFromMemory(ReadOnlyMemory<byte> data)
    {
        var ranges = new List<(int Start, int Length)>(capacity: 1024);

        var span = data.Span;
        int p = 0;
        while (p < span.Length)
        {
            int lineBreak = span[p..].IndexOf((byte)'\n');
            int lineEndExclusive = lineBreak >= 0 ? p + lineBreak : span.Length;

            int lineLength = lineEndExclusive - p;
            if (lineLength > 0 && span[p + lineLength - 1] == (byte)'\r')
            {
                lineLength -= 1;
            }

            if (lineLength > 0)
            {
                ranges.Add((p, lineLength));
            }

            if (lineBreak < 0)
            {
                break;
            }

            p = lineEndExclusive + 1;
        }

        var results = new CombatLogEvent?[ranges.Count];
        Parallel.For(0, ranges.Count, idx =>
        {
            var (start, length) = ranges[idx];
            var lineText = Encoding.UTF8.GetString(data.Span.Slice(start, length));
            results[idx] = GetCombatLogEvent(lineText);
        });

        var output = new List<CombatLogEvent>(results.Length);
        for (int idx = 0; idx < results.Length; idx++)
        {
            if (results[idx] is { } e)
            {
                output.Add(e);
            }
        }

        return output;
    }

    internal unsafe IReadOnlyList<CombatLogEvent> ParseParallelFromPointer(byte* basePtr, int length)
    {
        var ranges = new List<(int Start, int Length)>(capacity: 1024);

        int p = 0;
        while (p < length)
        {
            var remaining = new ReadOnlySpan<byte>(basePtr + p, length - p);
            int lineBreak = remaining.IndexOf((byte)'\n');
            int lineEndExclusive = lineBreak >= 0 ? p + lineBreak : length;

            int lineLength = lineEndExclusive - p;
            if (lineLength > 0 && *(basePtr + p + lineLength - 1) == (byte)'\r')
            {
                lineLength -= 1;
            }

            if (lineLength > 0)
            {
                ranges.Add((p, lineLength));
            }

            if (lineBreak < 0)
            {
                break;
            }

            p = lineEndExclusive + 1;
        }

        var results = new CombatLogEvent?[ranges.Count];
        Parallel.For(0, ranges.Count, idx =>
        {
            var (start, lineLength) = ranges[idx];
            var line = new ReadOnlySpan<byte>(basePtr + start, lineLength);
            var lineText = Encoding.UTF8.GetString(line);
            results[idx] = GetCombatLogEvent(lineText);
        });

        var output = new List<CombatLogEvent>(results.Length);
        for (int idx = 0; idx < results.Length; idx++)
        {
            if (results[idx] is { } e)
            {
                output.Add(e);
            }
        }

        return output;
    }

    /// <summary>
    /// Disposes the underlying file context.
    /// </summary>
    public void Dispose()
    {
        contextProvider.Dispose();
    }
}

public struct ParserError(Exception exception, string line)
{
    public Exception Exception = exception;
    public string Raw = line;
}