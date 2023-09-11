using System.Diagnostics;

namespace WoWCombatLogParser.Common.Events;

[DebuggerDisplay("({PermanentEnchantId}) ({TempEnchantId}) ({OnUseSpellEnchantId})")]
public class ItemEnchants : CombagLogEventComponent
{
    public int PermanentEnchantId { get; set; }
    public int TempEnchantId { get; set; }
    public int OnUseSpellEnchantId { get; set; }
}
