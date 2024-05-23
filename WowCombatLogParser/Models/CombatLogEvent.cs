using System;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static WoWCombatLogParser.IO.CombatLogFieldReader;

namespace WoWCombatLogParser.Models;

public abstract class CombatLogEvent : BaseCombatLogEvent
{
}
