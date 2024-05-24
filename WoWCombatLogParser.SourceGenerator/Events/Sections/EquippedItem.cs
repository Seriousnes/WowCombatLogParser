﻿using System.Collections.Generic;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Sections;

internal class EquippedItem : CombatLogEventComponent
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
