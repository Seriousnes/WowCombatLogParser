using System;
using System.Linq;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser;

public static class Constants
{
    public static readonly CombatLogVersion DefaultCombatLogVersion = Enum.GetValues(typeof(CombatLogVersion)).Cast<CombatLogVersion>().Max(x => x);
}
