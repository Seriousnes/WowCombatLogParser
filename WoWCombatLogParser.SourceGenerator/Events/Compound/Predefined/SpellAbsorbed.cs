using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Predefined;

[Affix("SPELL_ABSORBED")]
public class SpellAbsorbed : Predefined<Spell, Absorbed>
{
}
