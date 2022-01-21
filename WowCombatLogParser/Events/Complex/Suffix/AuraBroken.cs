namespace WoWCombatLogParser.Events
{
    [Suffix("_AURA_BROKEN")]
    public class AuraBroken : EventSection
    {
        public AuraType Type { get; set; }
    }

    [Suffix("_AURA_BROKEN_SPELL")]
    public class SpellAuraBroken : AbilityBase
    {
        public AuraType AuraType { get; set; }
    }

}
