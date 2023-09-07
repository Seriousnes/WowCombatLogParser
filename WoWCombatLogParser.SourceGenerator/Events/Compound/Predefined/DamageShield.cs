using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Affix("DAMAGE_SHIELD")]
public class DamageShield : Predefined<Spell, Damage>
{
}
