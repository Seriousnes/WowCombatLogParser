using WoWCombatLogParser.SourceGenerator.Events.Compound;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("PARTY_KILL")]
internal class PartyKill : CompoundEventSection
{
    public int OverkillAmount { get; set; }
}
