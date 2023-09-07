using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Models;

public interface IActor
{
    WowGuid Id { get; }
    string Name { get; }
    UnitType UnitType { get; }
}

public abstract class Actor
{
    public virtual WowGuid Id { get; set; }
    public virtual string Name { get; set; }
    public virtual UnitType UnitType { get; set; }
}

public class Player : Actor, IActor
{        
}
