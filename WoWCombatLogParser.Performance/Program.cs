using WoWCombatLogParser;

var context = new ApplicationContext();
context.CombatLogParser.Filename = @"C:\Users\Sean\source\repos\WoWCombatLogParser\WoWCombatLogParser.Tests\TestLogs\SingleFightCombatLog.txt";
var encounters = context.CombatLogParser.Scan().ToList();
await context.CombatLogParser.ParseAsync(encounters);