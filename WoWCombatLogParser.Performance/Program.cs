var parser = new WoWCombatLogParser.CombatLogParser(@"C:\Users\Sean\source\repos\WoWCombatLogParser\WoWCombatLogParser.Tests\TestLogs\SingleFightCombatLog.txt");
var encounters = parser.Scan().ToList();
Parallel.ForEachAsync(encounters, async (x, _) => await x.ParseAsync()).Wait();