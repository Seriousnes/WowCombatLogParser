using StackExchange.Profiling;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.IO;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser;

public interface ICombatLogParser
{
    CombatLogEvent? GetCombatLogEvent(string text);
    T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent;
    IEnumerable<Segment> GetSegments(string filename);
    IEnumerable<CombatLogEvent> ParseSegment(Segment segment);
    Dictionary<string, List<ParserError>> Errors { get; }
    MiniProfiler Profiler { get; }
}

public class CombatLogParser : ICombatLogParser
{
    private readonly CombatLogEventMapper mapper = new();
    private readonly Dictionary<string, string> segmentTypes;            
        
    public CombatLogParser() : this(CombatLogVersion.Dragonflight)
    {        
    }

    public CombatLogParser(CombatLogVersion combatLogVersion)
    {
        mapper.SetCombatLogVersion(combatLogVersion);

        var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
        segmentTypes = assemblyTypes
            .Where(x => x.Namespace == nameof(WoWCombatLogParser))
            .Where(x => x.IsAssignableTo(typeof(IFightStart)))
            .ToDictionary(k => k.GetCustomAttribute<DiscriminatorAttribute>()!.Value, v =>
            {
                var startAttributeValue = v.GetCustomAttribute<DiscriminatorAttribute>()!.Value;                
                var endAttributeValue = $"{startAttributeValue[..startAttributeValue.LastIndexOf('_')]}_END";

                return assemblyTypes.SingleOrDefault(x => x.GetCustomAttribute<DiscriminatorAttribute>() is DiscriminatorAttribute discriminator && discriminator.Value == endAttributeValue)?.GetCustomAttribute<DiscriminatorAttribute>()?.Value ?? string.Empty;
            });
    }

    public Dictionary<string, List<ParserError>> Errors { get; } = [];

    public MiniProfiler Profiler { get; } = MiniProfiler.DefaultOptions.StartProfiler("CombatLogParser")!;

    public CombatLogEvent? GetCombatLogEvent(string line) => GetCombatLogEvent<CombatLogEvent>(line);

    public T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent
    {
        try
        {
            return mapper.GetCombatLogEvent<T>(line);
        }
        catch (Exception exception)
        {
            var eventType = exception is CombatLogParserException parserException ? parserException.EventType : CombatLogFieldReader.ReadFields(line).EventType;
            if (!Errors.TryGetValue(eventType, out var errors))
            {
                Errors[eventType] = errors = [];
            }
            errors.Add(new(exception, line));
            return null;
        }
    }

    public IEnumerable<Segment> GetSegments(string filename)
    {
        using var stream = new FileStream(filename, new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.ReadWrite,
            BufferSize = StreamExtensions.GetBufferSize(filename),
            Options = FileOptions.RandomAccess
        });
        using (var outerStep = Profiler.Step("Get Segments"))
        {
            long i = -1;
            while ((i = stream.IndexOfAny(segmentTypes.Keys, i + 1, IndexMode.LineStart, out var match)) >= 0)
            {
                var endPosition = stream.IndexOf(segmentTypes[match.Value!], i, IndexMode.LineEnd);
                if (endPosition >= 0)
                {
                    using (var innerStep = Profiler.Step("Preparse start/end"))
                    {
                        // parse the start and end events immediately, and adjust the beginning & end positions accordingly
                        var startLine = stream.GetCurrentLine(ref i, IndexMode.LineStart)!;
                        var endLine = stream.GetCurrentLine(ref endPosition, IndexMode.LineEnd)!;

                        yield return new Segment(filename, i, endPosition - i)
                        {
                            Start = (IFightStart?)GetCombatLogEvent(startLine),
                            End = (IFightEnd?)GetCombatLogEvent(endLine)
                        };
                    }
                    
                }
                i = stream.IndexOf(Environment.NewLine, endPosition, IndexMode.LineEnd);
            }
        }
    }

    public IEnumerable<CombatLogEvent> ParseSegment(Segment segment)
    {
        using (var step = Profiler.Step("Parse Segment"))
        {
            foreach (var line in segment.Content)
            {
                var combatLogEvent = GetCombatLogEvent(line);
                if (combatLogEvent is { })
                    yield return combatLogEvent;
            }
        }
    }
}

public struct ParserError(Exception exception, string line)
{
    public Exception Exception = exception;
    public string Raw = line;
}