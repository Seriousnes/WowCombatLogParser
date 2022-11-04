using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{    
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
        public EventSections<Talent> ClassTalents { get; set; } = new EventSections<Talent>();
        public EventSections<Talent> PvPTalents { get; set; } = new EventSections<Talent>();
        public Powers Powers { get; set; } = new Powers();
        public NestedEventSections<EquippedItem> EquippedItems { get; set; } = new NestedEventSections<EquippedItem>();
        public EventSections<InterestingAura> InterestingAuras { get; set; } = new EventSections<InterestingAura>();
        public PvPStats PvPStats { get; set; } = new PvPStats();
    }

    public class Powers : EventSection
    {
        public Soulbind Soulbind { get; set; }
        public Covenant Covenant { get; set; }
        public NestedEventSections<AnimaPower> AnimaPowers { get; set; } = new NestedEventSections<AnimaPower>();
        public NestedEventSections<SoulbindTrait> SoulbindTraits { get; set; } = new NestedEventSections<SoulbindTrait>();
        public NestedEventSections<Conduit> Conduits { get; set; } = new NestedEventSections<Conduit>();
    }

    [DebuggerDisplay("{Id} @ {Count} (Maw Power ID: {MawPowerId})")]
    public class AnimaPower : EventSection
    {
        public int Id { get; set; }
        public int MawPowerId { get; set; }
        public int Count { get; set; }
    }

    public class SoulbindTrait : IdPart
    {
    }

    [DebuggerDisplay("{Id} (Ilvl: {ItemLevel})")]
    public class Conduit : IdPart
    {
        public int ItemLevel { get; set; }
    }
}
