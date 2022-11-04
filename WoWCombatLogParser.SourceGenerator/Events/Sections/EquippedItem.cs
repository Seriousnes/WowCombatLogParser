using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public class EquippedItem : EventSection
    {
        public int ItemId { get; set; }
        public int ItemLevel { get; set; }
        public ItemEnchants Enchantments { get; set; } = new ItemEnchants();
        public EventSections<BonusId> BonusIds { get; set; } = new EventSections<BonusId>();
        public EventSections<Gem> Gems { get; set; } = new EventSections<Gem>();
    }
}
