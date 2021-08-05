﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_REMOVED")]
    public class AuraRemoved : EventSection
    {
        public string AuraType { get; set; }
        public decimal Amount { get; set; }
    }
}
