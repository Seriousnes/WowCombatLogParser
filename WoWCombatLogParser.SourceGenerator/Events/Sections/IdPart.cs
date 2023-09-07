using System.Diagnostics;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models;

[DebuggerDisplay("{Id}")]
public abstract class IdPart<T> : Event
{
    public T Id { get; set; }
}

[DebuggerDisplay("{Id}")]
public class Talent : IdPart<int>
{
}

public class BonusId : IdPart<int>
{
}

public class Gem : IdPart<int>
{
}
