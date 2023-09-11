using System.Collections.Generic;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public class EquippedItem : CombagLogEventComponent
{
    public int ItemId { get; set; }
    public int ItemLevel { get; set; }
    [IsSingleDataField]
    public ItemEnchants Enchantments { get; set; } = new();
    [IsSingleDataField]
    public List<BonusId> BonusIds { get; set; } = new();
    [IsSingleDataField]
    public List<Gem> Gems { get; set; } = new();
}
