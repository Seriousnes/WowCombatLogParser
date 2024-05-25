using WoWCombatLogParser.Events;

namespace WoWCombatLogParser.Models;

public class Ability : CombatLogEventComponent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public SpellSchool School { get; set; }
    public override bool Equals(object? obj)
    {
        return obj is Ability ability && Id == ability.Id;
    }

    public override int GetHashCode() => base.GetHashCode();
}
