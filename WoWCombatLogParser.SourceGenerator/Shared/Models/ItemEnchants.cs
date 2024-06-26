﻿namespace WoWCombatLogParser;

[DebuggerDisplay("({PermanentEnchantId}) ({TempEnchantId}) ({OnUseSpellEnchantId})")]
public class ItemEnchants : CombatLogEventComponent
{
    public int PermanentEnchantId { get; set; }
    public int TempEnchantId { get; set; }
    public int OnUseSpellEnchantId { get; set; }
}
