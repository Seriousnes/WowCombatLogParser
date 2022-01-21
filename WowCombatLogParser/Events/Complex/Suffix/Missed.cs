﻿namespace WoWCombatLogParser.Events
{
    [Suffix("_MISSED")]
    [DebuggerDisplay("{MissType} {IsOffHand} {AmountMissed} {Critical}")]
    public class Missed : EventSection
    {
        public MissType MissType { get; set; }
        public bool IsOffHand { get; set; }
        public decimal AmountMissed { get; set; }
        public bool Critical { get; set; }
    }
}
