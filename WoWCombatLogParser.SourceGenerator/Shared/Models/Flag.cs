namespace WoWCombatLogParser;

[DebuggerDisplay("{UnitType} {Ownership} {Reaction} {Affiliation} {Special}")]
public readonly struct UnitFlag(uint value)
{
    public UnitTypeFlag UnitType { get; } = (UnitTypeFlag)(value & (uint)UnitTypeFlag.Mask);
    public OwnershipFlag Ownership { get; } = (OwnershipFlag)(value & (uint)OwnershipFlag.Mask);
    public ReactionFlag Reaction { get; } = (ReactionFlag)(value & (uint)ReactionFlag.Mask);
    public AffiliationFlag Affiliation { get; } = (AffiliationFlag)(value & (uint)AffiliationFlag.Mask);
    public SpecialFlag Special { get; } = (SpecialFlag)(value & (uint)SpecialFlag.Mask);
}
