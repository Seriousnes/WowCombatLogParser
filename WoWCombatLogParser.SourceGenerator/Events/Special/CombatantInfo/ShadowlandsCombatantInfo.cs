using System.Collections.Generic;
using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[CombatLogVersion(CombatLogVersion.Shadowlands)]
[Affix("COMBATANT_INFO")]
[DebuggerDisplay("{PlayerGuid} {Faction} {Strength} {Agility} {Stamina} {Intelligence} {Dodge} {Parry} {Block} {CritMelee} {CritRanged} {CritSpell} {Speed} {Lifesteel} {HasteMelee} {HasteRanged} {HasteSpell} {Avoidance} {Mastery} {VersatilityDamageDone} {VersatilityHealingDone} {VersatilityDamageTaken} {Armor} {CurrentSpecID} ")]
public class ShadowlandsCombatantInfo : CombatantInfo, ICombatantInfo
{
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Stamina { get; set; }
    public int Intelligence { get; set; }
    public int Dodge { get; set; }
    public int Parry { get; set; }
    public int Block { get; set; }
    public int CritMelee { get; set; }
    public int CritRanged { get; set; }
    public int CritSpell { get; set; }
    public int Speed { get; set; }
    public int Lifesteel { get; set; }
    public int HasteMelee { get; set; }
    public int HasteRanged { get; set; }
    public int HasteSpell { get; set; }
    public int Avoidance { get; set; }
    public int Mastery { get; set; }
    public int VersatilityDamageDone { get; set; }
    public int VersatilityHealingDone { get; set; }
    public int VersatilityDamageTaken { get; set; }
    public int Armor { get; set; }
    public int CurrentSpecID { get; set; }
    [IsSingleDataField]
    public List<Talent> ClassTalents { get; set; } = new();
    [IsSingleDataField]
    public List<Talent> PvPTalents { get; set; } = new();
    [IsSingleDataField]
    public Powers Powers { get; set; } = new Powers();
    [IsSingleDataField]
    public List<EquippedItem> EquippedItems { get; set; } = new();
    [IsSingleDataField]
    public List<InterestingAura> InterestingAuras { get; set; } = new();
    public PvPStats PvPStats { get; set; } = new PvPStats();
}

public class Powers : Event
{
    public Soulbind Soulbind { get; set; }
    public Covenant Covenant { get; set; }
    [IsSingleDataField]
    public List<AnimaPower> AnimaPowers { get; set; } = new();
    [IsSingleDataField]
    public List<SoulbindTrait> SoulbindTraits { get; set; } = new();
    [IsSingleDataField]
    public List<Conduit> Conduits { get; set; } = new();
}

[DebuggerDisplay("{Id} @ {Count} (Maw Power ID: {MawPowerId})")]
public class AnimaPower : Event
{
    public int Id { get; set; }
    public int MawPowerId { get; set; }
    public int Count { get; set; }
}

public class SoulbindTrait : IdPart<int>
{
}

[DebuggerDisplay("{Id} (Ilvl: {ItemLevel})")]
public class Conduit : IdPart<int>
{
    public int ItemLevel { get; set; }
}
