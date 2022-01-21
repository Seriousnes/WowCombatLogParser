using System;

namespace WoWCombatLogParser.Events
{
    [Affix("COMBATANT_INFO")]
    [DebuggerDisplay("{PlayerGuid} {Faction} {Strength} {Agility} {Stamina} {Intelligence} {Dodge} {Parry} {Block} {CritMelee} {CritRanged} {CritSpell} {Speed} {Lifesteel} {HasteMelee} {HasteRanged} {HasteSpell} {Avoidance} {Mastery} {VersatilityDamageDone} {VersatilityHealingDone} {VersatilityDamageTaken} {Armor} {CurrentSpecID} ")]
    public class CombatantInfo : EventSection
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
        public EventSections<Talent> ClassTalents { get; set; } = new();
        public EventSections<Talent> PvPTalents { get; set; } = new();
        public Powers Powers { get; set; } = new();
        public NestedEventSections<EquippedItem> EquippedItems { get; set; } = new();
        public EventSections<InterestingAura> InterestingAuras { get; set; } = new();
        public PvPStats PvPStats { get; set; } = new();        
    }

    [DebuggerDisplay("{TalentId}")]
    public class Talent : EventSection
    {
        public int TalentId { get; set; }
    }

    [DebuggerDisplay("{Id}")]
    public abstract class IdPart : EventSection
    {
        public int Id { get; set; }

    }

    public class Powers : EventSection
    {
        public Soulbind Soulbind { get; set; }
        public Covenant Covenant { get; set; }
        public NestedEventSections<AnimaPower> AnimaPowers { get; set; } = new();
        public NestedEventSections<SoulbindTrait> SoulbindTraits { get; set; } = new();
        public NestedEventSections<Conduit> Conduits { get; set; } = new();
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

    public class EquippedItem : EventSection
    {
        public int ItemId { get; set; }
        public int ItemLevel { get; set; }
        public ItemEnchants Enchantments { get; set; } = new();
        public EventSections<BonusId> BonusIds { get; set; } = new();
        public EventSections<Gem> Gems { get; set; } = new();

    }

    [DebuggerDisplay("({PermanentEnchantId}) ({TempEnchantId}) ({OnUseSpellEnchantId})")]
    public class ItemEnchants : EventSection
    {
        public int PermanentEnchantId { get; set; }
        public int TempEnchantId { get; set; }
        public int OnUseSpellEnchantId { get; set; }
    }

    public class BonusId : IdPart
    {
    }

    public class Gem : IdPart
    {
    }

    public class InterestingAura : EventSection
    {
        public WowGuid PlayerId { get; set; }
        public int AuraId { get; set; }
    }

    public class PvPStats : EventSection
    {
        public int HonorLevel { get; set; }
        public int Season { get; set; }
        public int Rating { get; set; }
        public int Tier { get; set; }
    }
}
