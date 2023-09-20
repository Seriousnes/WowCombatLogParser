using System;

namespace WoWCombatLogParser.Models;

public class Segment
{
    internal IParserContext ParserContext { get; set; }

    public Segment(string value)
    {
        Content.AddRange(value.Split(Environment.NewLine));
    }

    public List<string> Content { get; set; } = new();
}