using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_DISPEL")]
    public class Dispel : SuffixAbilityBase
    {
        public AuraType AuraType { get; set; }
    }

    [Suffix("_STOLEN")]
    public class Stolen : SuffixAbilityBase
    {
        public AuraType AuraType { get; set; }
    }

}
