namespace WoWCombatLogParser.Events
{
    [Suffix("_AURA_REFRESH")]
    public class AuraRefreshed : EventSection
    {
        public AuraType Type { get; set; }
    }
}
