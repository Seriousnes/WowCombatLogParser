namespace WoWCombatLogParser.Events
{
    [Suffix("_DAMAGE")]
    [DebuggerDisplay("{RawAmount} {Overkill} {School} {Resisted} {Blocked} {Absorbed} {Critical} {Glancing} {Crushing} {IsOffHand}")]
    public class Damage : AdvancedLoggingDetailsBase<decimal>
    {
        public decimal UnmitigatedAmount { get; set; }
        public bool IsOverkill { get; set; }
        public SpellSchool School { get; set; }
        public decimal Resisted { get; set; }
        public decimal Blocked { get; set; }
        public decimal Absorbed { get; set; }
        public bool Critical { get; set; }
        public bool Glancing { get; set; }
        public bool Crushing { get; set; }
        public bool IsOffHand { get; set; }
    }


    /// <remarks>
    /// Inherited events populate properties in order of inheritance. If this event inherits from Damage, it will populate the Damage properties before the advanced logging properties
    /// </remarks>
    [Suffix("_DAMAGE_LANDED")]    
    [DebuggerDisplay("{RawAmount} {Overkill} {School} {Resisted} {Blocked} {Absorbed} {Critical} {Glancing} {Crushing} {IsOffHand}")]
    public class DamageLanded : AdvancedLoggingDetailsBase<decimal>
    {
        public decimal UnmitigatedAmount { get; set; }
        public decimal IsOverkill { get; set; }
        public SpellSchool School { get; set; }
        public decimal Resisted { get; set; }
        public decimal Blocked { get; set; }
        public decimal Absorbed { get; set; }
        public bool Critical { get; set; }
        public bool Glancing { get; set; }
        public bool Crushing { get; set; }
        public bool IsOffHand { get; set; }
    }
}
