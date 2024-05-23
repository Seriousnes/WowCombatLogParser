using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("PARTY_KILL")]
public class PartyKill : CompoundEventSection
{
    public int OverkillAmount { get; set; }
}
