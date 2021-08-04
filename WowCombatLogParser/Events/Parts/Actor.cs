﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    public class Actor : EventSection
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public long Flags { get; set; }
        public long RaidFlags { get; set; }
    }
}
