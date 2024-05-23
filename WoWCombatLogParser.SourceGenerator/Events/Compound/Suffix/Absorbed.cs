using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

public class Absorbed
{
    public Actor AbsorbedBy { get; set; }
    public Ability ExtraSpell { get; set; }
    public int AbsorbedAmount { get; set; }
    public decimal UnmitigatedAmount { get; set; }
}
