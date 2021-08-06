using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Simple
{
    [Affix("COMBATANT_INFO")]
    public class CombatantInfo : IEventSection
    {
        public WowGuid PlayerGuid { get; set; } = new();
        public Faction Faction { get; set; }
        public decimal Strength { get; set; }
        public decimal Agility { get; set; }
        public decimal Stamina { get; set; }
        public decimal Intelligence { get; set; }
        public decimal Dodge { get; set; }
        public decimal Parry { get; set; }
        public decimal Block { get; set; }
        public decimal CritMelee { get; set; }
        public decimal CritRanged { get; set; }
        public decimal CritSpell { get; set; }
        public decimal Speed { get; set; }
        public decimal Lifesteel { get; set; }
        public decimal HasteMelee { get; set; }
        public decimal HasteRanged { get; set; }
        public decimal HasteSpell { get; set; }
        public decimal Avoidance { get; set; }
        public decimal Mastery { get; set; }
        public decimal VersatilityDamageDone { get; set; }
        public decimal VersatilityHealingDone { get; set; }
        public decimal VersatilityDamageTaken { get; set; }
        public decimal Armor { get; set; }
        public decimal CurrentSpecID { get; set; }
        //public Talents Talents { get; set; } = new Talents();
        //
        //public decimal VersatilityHealingDone { get; set; }
        //
        //public decimal VersatilityHealingDone { get; set; }
        //
        //public decimal VersatilityHealingDone { get; set; }
    }
}
