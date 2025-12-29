using System.Collections.Generic;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special.CombatantInfoEvents;

[CombatLogVersion(CombatLogVersion.Dragonflight)]
[Affix("COMBATANT_INFO")]
[DebuggerDisplay("{PlayerGuid} {Faction} {Strength} {Agility} {Stamina} {Intelligence} {Dodge} {Parry} {Block} {CritMelee} {CritRanged} {CritSpell} {Speed} {Lifesteel} {HasteMelee} {HasteRanged} {HasteSpell} {Avoidance} {Mastery} {VersatilityDamageDone} {VersatilityHealingDone} {VersatilityDamageTaken} {Armor} {CurrentSpecID} ")]
internal abstract class DragonflightCombatantInfo : CombatantInfo, ICombatantInfo
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
    public List<DragonflightTalent> ClassTalents { get; set; }
    [IsSingleDataField]
    public List<Sections.Talent> PvPTalents { get; set; }
    [IsSingleDataField]
    public List<Sections.EquippedItem> EquippedItems { get; set; }
    [IsSingleDataField]
    [KeyValuePair]
    public List<Sections.InterestingAura> InterestingAuras { get; set; }
    public Sections.PvPStats PvPStats { get; set; }
}