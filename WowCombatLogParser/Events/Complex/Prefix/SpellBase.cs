using WoWCombatLogParser.Events.Parts;

namespace WoWCombatLogParser.Events.Complex
{
    [DebuggerDisplay("{Spell}")]
    public class SpellBase : EventSection
    {
        public Ability Spell { get; } = new();
    }
}
