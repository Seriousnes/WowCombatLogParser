namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("{UnitType} {Ownership} {Reaction} {Affiliation} {Special}")]
    public readonly struct UnitFlag
    {
        public UnitFlag(uint value)
        {
            UnitType = (UnitTypeFlag)(value & (uint)UnitTypeFlag.Mask);
            Ownership = (OwnershipFlag)(value & (uint)OwnershipFlag.Mask);
            Reaction = (ReactionFlag)(value & (uint)ReactionFlag.Mask);
            Affiliation = (AffiliationFlag)(value & (uint)AffiliationFlag.Mask);
            Special = (SpecialFlag)(value & (uint)SpecialFlag.Mask);
        }

        public UnitTypeFlag UnitType { get; }
        public OwnershipFlag Ownership { get; }
        public ReactionFlag Reaction { get; }
        public AffiliationFlag Affiliation { get; }
        public SpecialFlag Special { get; }
    }
}
