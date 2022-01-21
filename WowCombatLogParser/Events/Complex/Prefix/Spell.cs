namespace WoWCombatLogParser.Events
{
    [Prefix("SPELL")]
    [SuffixNotAllowed(typeof(DamageLanded))]
    public class Spell : AbilityBase
    {
    }
}
