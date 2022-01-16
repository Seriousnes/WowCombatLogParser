using WoWCombatLogParser.Events.Parts;

namespace WoWCombatLogParser.Events.Complex
{
    [DebuggerDisplay("{Spell}")]
    public class SpellBase : Part
    {
        public Ability Spell { get; } = new();
    }
}
