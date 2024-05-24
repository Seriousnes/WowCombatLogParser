using System.Diagnostics;

namespace WoWCombatLogParser.SourceGenerator.Events.Sections;

[DebuggerDisplay("({PermanentEnchantId}) ({TempEnchantId}) ({OnUseSpellEnchantId})")]
internal class ItemEnchants : CombatLogEventComponent
{
    public int PermanentEnchantId { get; set; }
    public int TempEnchantId { get; set; }
    public int OnUseSpellEnchantId { get; set; }
}
