﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WoWCombatLogParser.Sections;

namespace WoWCombatLogParser.Utility
{
    public static class WowheadExtensions
    {
        public static string GetWowheadLink(this EquippedItem item) => $"https://www.wowhead.com/item={item.ItemId}/?bonus={string.Join(':', item.BonusIds.Select(x => x.Id))}";
    }
}
