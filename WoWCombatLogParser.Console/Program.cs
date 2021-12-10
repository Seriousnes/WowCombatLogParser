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
            var segments = ParseCombatLogSegments(debugFile).Take(2).ToList();

            Console.WriteLine("Parsing complete, press any key to close");
            Console.ReadKey();
        }
    }
}
