namespace WoWCombatLogParser.Events
{
    [Prefix("SWING")]
    [SuffixAllowed(typeof(Damage), typeof(DamageLanded), typeof(Missed))]
    public class Swing : EventSection
    {
    }
}
