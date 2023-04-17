using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net.Http.Headers;
using WoWCombatLogParser;

namespace WowCombatLogParser.App
{
    internal class Program
    {
        static readonly string baseDirectory = @"E:\Games\World of Warcraft\_retail_\Logs";

        static async Task Main(string[] args)
        {
            var log = Directory.GetFiles(baseDirectory, "WowCombatLog*.txt")
                .Select(x => new FileInfo(x))
                .OrderByDescending(x => x.LastAccessTime)
                .Select(x => x.FullName)
                .FirstOrDefault();

            var context = new ApplicationContext();

            var sw = new Stopwatch();            
            sw.Start();
            
            var parser = context.CombatLogParser;
            parser.Filename = log;
            //var events = parser.GetCombatLogEvents();
            //Console.WriteLine($"{events.Count()} scanned.");
            
            sw.Stop();
            Console.WriteLine($"Time taken: {sw.Elapsed}");
            Console.ReadKey();
        }
    }
}