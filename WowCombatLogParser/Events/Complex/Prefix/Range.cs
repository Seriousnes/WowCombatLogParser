﻿namespace WoWCombatLogParser.Events
{
    [Prefix("RANGE")]
    [SuffixAllowed(typeof(Damage), typeof(Missed))]
    [SuffixNotAllowed(typeof(DamageLanded))]
    public class Range : AbilityBase
    {
    }
}
