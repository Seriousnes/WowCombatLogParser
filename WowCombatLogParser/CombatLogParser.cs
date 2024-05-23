using System;
using System.IO;
using WoWCombatLogParser.IO;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser;

public interface ICombatLogParser
{
    IParserContext? ParserContext { get; set; }
    IEnumerable<Segment> GetSegments(string filename);
    CombatLogEvent? ParseLine(string text);
    IEnumerable<CombatLogEvent> ParseSegment(Segment segment);
    Dictionary<string, List<ParserError>> Errors { get; }
}

public class CombatLogParser : ICombatLogParser
{
    public IParserContext? ParserContext { get; set; }
    public Dictionary<string, List<ParserError>> Errors { get; } = [];

    public CombatLogEvent? ParseLine(string line)
    {
        var data = CombatLogFieldReader.ReadFields(line);
        try
        {
            return ParserContext?.EventGenerator.GetCombatLogEvent<CombatLogEvent>(data);
        }
        catch (Exception exception)
        {
            var eventType = exception is CombatLogParserException parserException ? parserException.EventType : data.EventType;
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
        //using var sr = new StreamReader(fs);        
        //var content = sr.ReadToEnd();
        //string content;

        long i = -1;
        while ((i = stream.IndexOf("ENCOUNTER_START", i + 1, IndexMode.LineStart)) >= 0)
        {
            var endPosition = stream.IndexOf("ENCOUNTER_END", i, IndexMode.LineEnd);
            if (endPosition >= 0)
            {
                yield return new Segment(filename, i, endPosition - i) { ParserContext = ParserContext };
            }
            i = stream.IndexOf(Environment.NewLine, i, IndexMode.LineEnd);
        }
    }

    public IEnumerable<CombatLogEvent> ParseSegment(Segment segment)
    {
        foreach (var line in segment.Content)
        {
            var combatLogEvent = ParseLine(line);
            if (combatLogEvent is { })
                yield return combatLogEvent;
        }
    }
}

public struct ParserError(Exception exception, string line)
{
    public Exception Exception = exception;
    public string Raw = line;
}