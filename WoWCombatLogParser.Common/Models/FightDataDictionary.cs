using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WoWCombatLogParser.Common.Models;

public class FightDataDictionary : ConcurrentDictionary<Type, List<IKey>>
{
}
