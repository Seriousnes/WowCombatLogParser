using System.IO;
using System.Threading.Tasks;
using System;
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
        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs);
        var content = sr.ReadToEnd();

        int i = -1;
        while ((i = content.IndexOf("ENCOUNTER_START", i + 1, IndexMode.LineStart)) >= 0)
        {
            var endPosition = content.IndexOf("ENCOUNTER_END", i, IndexMode.LineEnd);
            if (endPosition >= 0)
            {
                yield return new Segment(content.Substring(i, endPosition - i)) { ParserContext = ParserContext };
            }
            i = content.IndexOf(Environment.NewLine, i, IndexMode.LineEnd);
        }
    }

    public IEnumerable<CombatLogEvent> ParseSegment(Segment segment)
    {
        foreach (var line in segment.Content)
            yield return ParseLine(line);
    }
}
