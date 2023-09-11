using System.Diagnostics;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models;

[DebuggerDisplay("{Id} {Name} {School}")]
public class Ability : CombagLogEventComponent, IKey
{
    [Key(3)]
    public int Id { get; set; }
    public string Name { get; set; }
    public SpellSchool School { get; set; }
    public bool EqualsKey(IKey key)
    {
        return key is Ability ability && Id == ability.Id;
    }
}
