using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_HEAL")]
    [DebuggerDisplay("{Amount} {Overhealing} {Absorbed} {Critical}")]
    public class Healing : AdvancedLoggingDetailsBase<int>
    {
        public int Overhealing { get; set; }
        public int Absorbed { get; set; }
        public bool Critical { get; set; }
    }
}
