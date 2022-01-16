using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_DAMAGE")]
    [DebuggerDisplay("{RawAmount} {Overkill} {School} {Resisted} {Blocked} {Absorbed} {Critical} {Glancing} {Crushing} {IsOffHand}")]
    public class Damage : AdvancedLoggingDetailsBase<decimal>
    {
        public decimal RawAmount { get; set; }
        public decimal Overkill { get; set; }
        public SpellSchool School { get; set; }
        public decimal Resisted { get; set; }
        public decimal Blocked { get; set; }
        public decimal Absorbed { get; set; }
        public bool Critical { get; set; }
        public bool Glancing { get; set; }
        public bool Crushing { get; set; }
        public bool IsOffHand { get; set; }
    }
}
