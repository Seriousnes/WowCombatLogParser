namespace WoWCombatLogParser;

[DebuggerDisplay("{Id}")]
public class IdPart<T> : CombatLogEventComponent
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

public class DragonflightTalent : CombatLogEventComponent
{
    public int Id { get; set; }
    public int Unknown { get; set; }
    public int Ranks { get; set; }
}