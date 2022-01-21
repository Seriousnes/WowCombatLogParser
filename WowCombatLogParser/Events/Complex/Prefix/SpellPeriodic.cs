namespace WoWCombatLogParser.Events
{
    [Prefix("SPELL_PERIODIC")]
    [SuffixAllowed(typeof(Healing), typeof(Damage), typeof(Energize), typeof(Missed))]
    public class SpellPeriodic : AbilityBase
    {
    }
}
