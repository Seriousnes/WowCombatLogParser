using System;
using System.Linq;
using WoWCombatLogParser.Events.Complex;
using WoWCombatLogParser.Models;
using static WoWCombatLogParser.CombatLogParser;

namespace WoWCombatLogParser
{
    class Program
    {
        private static string debugFile = @"C:\Users\Sean\source\repos\WoWCombatLogParser\WoWCombatLogParser.Tests\TestLogs\WoWCombatLog.txt";
        static void Main(string[] args)
        {
            var events = ParseCombatLog(debugFile).ToList();
            var spellDamage = events.OfType<CombatLogEvent<Spell, Damage>>().ToList();

            Console.WriteLine("Parsing complete, press any key to close");
            Console.ReadKey();
        }
    }
}
