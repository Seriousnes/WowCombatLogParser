using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("SPELL_EMPOWER_INTERRUPT")]
public class SpellEmpowerInterrupt : CompoundEventSection, IEmpowerFinish
{
    public Ability Spell { get; set; }
    public int Stage { get; set; }
}
