using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using WoWCombatLogParser;

namespace WowCombatLogParser.App
{
    internal class Program
    {
        static readonly string baseDirectory = @"E:\Games\World of Warcraft\_retail_\Logs";

        static async Task Main(string[] args)
        {
            var context = new ApplicationContext();
            var log = Directory.GetFiles(baseDirectory, "WowCombatLog*.txt")
                .Select(x => new FileInfo(x))
                .OrderByDescending(x => x.LastAccessTime)
                .Select(x => x.FullName)
                .First();

            var sw = new Stopwatch();            
            sw.Start();

            //using var fs = new FileStream(log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            context.CombatLogParser.Filename = log;
            var fights = context.CombatLogParser.Scan(true);
            var firstKill = fights.FirstOrDefault(x => x is Boss && x.IsSuccess);
            context.CombatLogParser.Parse(firstKill);

            //var events = parser.GetCombatLogEvents();
            //Console.WriteLine($"{events.Count()} scanned.");
            
            sw.Stop();
            Console.WriteLine($"Time taken: {sw.Elapsed}");
            Console.ReadKey();
        }
    }
}