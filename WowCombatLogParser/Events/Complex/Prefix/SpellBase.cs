using Spells = WoWCombatLogParser.Events.Parts;

namespace WoWCombatLogParser.Events.Complex
{
    [DebuggerDisplay("{Spell}")]
    public class SpellBase : IEventSection
    {
        public Spells.Spell Spell { get; } = new Spells.Spell();
    }
}
