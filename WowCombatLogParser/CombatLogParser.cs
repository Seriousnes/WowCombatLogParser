using System;
using System.IO;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser;

public interface ICombatLogParser
{
    IParserContext ParserContext { get; set; }
    IEnumerable<Segment> GetSegments(string filename);
    CombatLogEvent ParseLine(string text);
    IEnumerable<CombatLogEvent> ParseSegment(Segment segment);
}

public class CombatLogParser : ICombatLogParser
{
    public IParserContext ParserContext { get; set; }
    
    public CombatLogEvent ParseLine(string line)
    {
        return ParserContext.EventGenerator.GetCombatLogEvent<CombatLogEvent>(line);
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
            yield return ParseLine(line);
    }
}
