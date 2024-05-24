global using System.Collections.Generic;
global using System.Diagnostics;

global using WoWCombatLogParser.Events;
global using WoWCombatLogParser.Models;
global using WoWCombatLogParser.Sections;

global using static WoWCombatLogParser.Utility.Conversion;

global using AffixAttribute = WoWCombatLogParser.SourceGenerator.Models.AffixAttribute;
global using PrefixAttribute = WoWCombatLogParser.SourceGenerator.Models.PrefixAttribute;
global using SuffixAttribute = WoWCombatLogParser.SourceGenerator.Models.SuffixAttribute;
global using IsSingleDataFieldAttribute = WoWCombatLogParser.SourceGenerator.Models.IsSingleDataFieldAttribute;
global using NonDataAttribute = WoWCombatLogParser.SourceGenerator.Models.NonDataAttribute;