using System;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public abstract class SuffixAbilityBase : EventSection
    {
        public Ability ExtraSpell { get; set; } = new();
    }
}
