using System.Diagnostics;

namespace WoWCombatLogParser.SourceGenerator.Events.Sections;

[DebuggerDisplay("{Id}")]
internal abstract class IdPart<T> : CombatLogEventComponent
{
    public T Id { get; set; }
}

[DebuggerDisplay("{Id}")]
internal class Talent : IdPart<int>
{
}

internal class BonusId : IdPart<int>
{
}

internal class Gem : IdPart<int>
{
}
