using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_START")]
public class Start : CombatLogEventComponent
{
}

[Suffix("_END")]
public class End : CombatLogEventComponent, IEmpowerFinish
{
    public int Stage { get; set; }
}
