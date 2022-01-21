namespace WoWCombatLogParser.Events
{
    [Suffix("_DISPEL")]
    public class Dispel : AbilityBase
    {
        public AuraType AuraType { get; set; }
    }

    [Suffix("_STOLEN")]
    public class Stolen : AbilityBase
    {
        public AuraType AuraType { get; set; }
    }

}
