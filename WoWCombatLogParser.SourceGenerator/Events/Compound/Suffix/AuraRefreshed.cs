using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_AURA_REFRESH")]
    public class AuraRefreshed : EventSection, IAura
    {
        public AuraType AuraType { get; set; }
    }
}
