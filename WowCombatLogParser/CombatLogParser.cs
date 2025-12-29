using StackExchange.Profiling;
using System;
using System.IO;
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
/// <item><description>Create a parser with an <see cref="ICombatLogEventMapper"/> and an <see cref="ICombatLogSegmentProvider"/>.</description></item>
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
    /// Parses all events within the provided segment.
    /// </summary>
    /// <param name="segment">A segment obtained from <see cref="GetSegments"/>.</param>
    /// <returns>A list of parsed events.</returns>
    IReadOnlyList<CombatLogEvent> ParseSegment(Segment segment);

    /// <summary>
    /// Asynchronously parses all events within the provided segment.
    /// </summary>
    /// <param name="segment">A segment obtained from <see cref="GetSegments"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task producing a list of parsed events.</returns>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is cancelled.</exception>
    ValueTask<IReadOnlyList<CombatLogEvent>> ParseSegmentAsync(Segment segment, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Gets the profiler used to record parsing steps.
    /// </summary>
    /// <remarks>
    /// This property is intended for diagnostics and may be removed in a future version.
    /// </remarks>
    MiniProfiler Profiler { get; }
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
    private readonly ICombatLogSegmentProvider segmentProvider;
    private ICombatLogFileContext? fileContext;
    private string? filePath;
    private readonly object errorsGate = new();

    /// <summary>
    /// Creates a new parser.
    /// </summary>
    /// <param name="mapper">Maps raw combat log lines to concrete event types.</param>
    /// <param name="segmentProvider">Provides segmentation and segment loading for a combat log file.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="segmentProvider"/> is <see langword="null"/>.</exception>
    public CombatLogParser(ICombatLogEventMapper mapper, ICombatLogSegmentProvider segmentProvider)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.segmentProvider = segmentProvider ?? throw new ArgumentNullException(nameof(segmentProvider));

        this.mapper.SetCombatLogVersion(Constants.DefaultCombatLogVersion);
    }

    /// <inheritdoc />
    public Dictionary<string, List<ParserError>> Errors { get; } = [];

    /// <inheritdoc />
    public MiniProfiler Profiler { get; } = MiniProfiler.DefaultOptions.StartProfiler("CombatLogParser")!;

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
            lock (errorsGate)
            {
                if (!Errors.TryGetValue(eventType, out var errors))
                {
                    Errors[eventType] = errors = [];
                }
                errors.Add(new(exception, line));
            }
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

        Errors.Clear();
        this.filePath = filePath;

        fileContext?.Dispose();
        fileContext = null;

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

        fileContext = segmentProvider.Open(filePath, GetCombatLogEvent);
    }

    /// <inheritdoc />
    public IReadOnlyList<Segment> GetSegments()
    {
        if (filePath is null)
            throw new InvalidOperationException("SetFilePath must be called before GetSegments.");

        if (fileContext is null)
            throw new InvalidOperationException("File context has not been initialized.");

        using var step = Profiler.Step("Get Segments");
        return fileContext.Segments;
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<Segment>> GetSegmentsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(GetSegments());
    }

    /// <inheritdoc />
    public IReadOnlyList<CombatLogEvent> ParseSegment(Segment segment)
    {
        using var step = Profiler.Step("Parse Segment");
        return segmentProvider.LoadSegment(segment);
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<CombatLogEvent>> ParseSegmentAsync(Segment segment, CancellationToken cancellationToken = default)
    {
        using var step = Profiler.Step("Parse Segment (Async)");
        return segmentProvider.LoadSegmentAsync(segment, cancellationToken);
    }

    /// <inheritdoc />
    public IReadOnlyList<CombatLogEvent> LoadEvents(long startOffset, int length)
    {
        if (fileContext is null)
            throw new InvalidOperationException("File context has not been initialized.");

        using var step = Profiler.Step("Load Events");
        return fileContext.LoadEvents(startOffset, length);
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<CombatLogEvent>> LoadEventsAsync(long startOffset, int length, CancellationToken cancellationToken = default)
    {
        if (fileContext is null)
            throw new InvalidOperationException("File context has not been initialized.");

        using var step = Profiler.Step("Load Events (Async)");
        return fileContext.LoadEventsAsync(startOffset, length, cancellationToken);
    }

    /// <summary>
    /// Disposes the underlying file context.
    /// </summary>
    public void Dispose()
    {
        fileContext?.Dispose();
        fileContext = null;
    }
}

public struct ParserError(Exception exception, string line)
{
    public Exception Exception = exception;
    public string Raw = line;
}