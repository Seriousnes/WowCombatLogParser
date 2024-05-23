using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("EMOTE")]
public class Emote
{
    public Unit Source { get; set; } = new();
    public Unit Destination { get; set; } = new();
    public string Text { get; set; }
}
