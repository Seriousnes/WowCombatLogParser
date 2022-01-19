using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Events.Special
{
    [Affix("COMBATANT_INFO")]
    [DebuggerDisplay("{PlayerGuid} {Faction} {Strength} {Agility} {Stamina} {Intelligence} {Dodge} {Parry} {Block} {CritMelee} {CritRanged} {CritSpell} {Speed} {Lifesteel} {HasteMelee} {HasteRanged} {HasteSpell} {Avoidance} {Mastery} {VersatilityDamageDone} {VersatilityHealingDone} {VersatilityDamageTaken} {Armor} {CurrentSpecID} ")]
    public class CombatantInfo : Part
    {
        public WowGuid PlayerGuid { get; set; }
        public Faction Faction { get; set; }
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
        public PartList<Talent> ClassTalents { get; set; } = new(")");
        public PartList<Talent> PvPTalents { get; set; } = new(")");
        public Powers Powers { get; set; } = new();
    }    

    [DebuggerDisplay("{TalentId}")]
    public class Talent : Part
    {
        public int TalentId { get; set; }
    }

    public class Powers : Part
    {
        public Soulbind Soulbind { get; set; }
        public Covenant Covenant { get; set; }
        public PartList<AnimaPower> AnimaPowers { get; set; } = new("]", ")]");
        public PartList<SoulbindTrait> SoulbindTraits { get; set;} = new("]", ")]");
        public PartList<Conduit> Conduits { get; set; } = new("]]", ")]]");
    }    

    public class AnimaPower : Part
    {
        public int Id { get; set; }
        public int MawPowerId { get; set; }
        public int Count { get; set; }
    }

    public class SoulbindTrait : Part
    {
        public int Id { get; set; }
    }

    public class Conduit : Part
    {
        public int Id { get; set; }
        public int ItemLevel { get; set; }        
    }
}
