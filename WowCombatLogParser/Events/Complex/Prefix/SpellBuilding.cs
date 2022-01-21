namespace WoWCombatLogParser.Events
{
    [Prefix("SPELL_BUILDING")]
    [SuffixAllowed(typeof(Damage), typeof(Healing))]
    public class SpellBuilding : AbilityBase
    {
    }
}
