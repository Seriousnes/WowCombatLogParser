using System;
using System.Linq;
using WoWCombatLogParser.Models;
using static WoWCombatLogParser.CombatLogParser;

namespace WoWCombatLogParser
{
    class Program
    {
        private static string debugFile = @"C:\Users\Sean\source\repos\WoWCombatLogParser\WoWCombatLogParser.Tests\TestLogs\SingleFightCombatLog.txt";
        static void Main(string[] args)
        {
            Encounter encounter = ParseCombatLogSegments(debugFile).FirstOrDefault();
            Console.WriteLine($"Encounter has {encounter.Details.Combatants.Count()} combatants and {encounter.Events.Count()} events");
            Console.WriteLine("Parsing complete, press any key to close");
            Console.ReadKey();
        }
    }
}
