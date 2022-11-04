using System.Diagnostics;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models
{
    [DebuggerDisplay("{Id}")]
    public abstract class IdPart : EventSection
    {
        public int Id { get; set; }
    }

    [DebuggerDisplay("{Id}")]
    public class Talent : IdPart
    {
    }

    public class BonusId : IdPart
    {
    }

    public class Gem : IdPart
    {
    }
}
