using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public abstract class PredefinedBase : EventSection
    {
    }

    public abstract class Predefined<T1> : PredefinedBase
    {
    }

    public abstract class Predefined<T1, T2> : PredefinedBase
    {
    }
}
