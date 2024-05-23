using System.Collections.Generic;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public class EquippedItem : CombatLogEventComponent
{
    public int ItemId { get; set; }
    public int ItemLevel { get; set; }
    [IsSingleDataField]
    public ItemEnchants Enchantments { get; set; } = new();
    [IsSingleDataField]
    public List<BonusId> BonusIds { get; set; } = [];
    [IsSingleDataField]
    public List<Gem> Gems { get; set; } = [];
}
