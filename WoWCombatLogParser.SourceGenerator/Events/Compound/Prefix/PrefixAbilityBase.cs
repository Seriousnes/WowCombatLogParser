using System;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public abstract class PrefixAbilityBase : EventSection
    {
        public Ability Spell { get; } = new Ability();
    }
}
