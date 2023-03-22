using System;
using System.Net.Http.Headers;
using WoWCombatLogParser;

namespace WowCombatLogParser.App
{
    internal class Program
    {
        // WoWCombatLog-031423_155722.txt

        static async Task Main(string[] args)
        {
            var log = args.FirstOrDefault();
            if (!File.Exists(log))
            {
                Console.WriteLine($"Log file {log} not found.");
                return;
            }

            var videoStart = DateTime.Parse($"{DateTime.Today:yyyy-MM-dd} 19:01:22").Add(TimeSpan.FromSeconds(15));

            var context = new ApplicationContext();
            var parser = context.CombatLogParser;

            parser.Filename = log;
            var fights = parser.Scan(quickScan: true);            

            string description = "";            
            foreach (var boss in fights.GroupBy(x => x.Name))
            {
                int i = 0;
                description += $"{boss.First().Name}\n{new string('=', 20)}\n";
                foreach (var fight in boss)
                {                    
                    var fightDetails = fight.GetDetails();
                    var timestamp = $"{fightDetails.Time - videoStart:hh\\:mm\\:ss}";

                    string line;
                    if (fightDetails.Result == "Kill")
                        line = $"Kill - {timestamp}";
                    else
                    {
                        i++;
                        line = $"Wipe {i} - {timestamp}";
                    }

                    Console.WriteLine(line);
                    description += $"{line}\n";
                }

                description += '\n';
            }

            TextCopy.ClipboardService.SetText(description);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        private static void GetVideoTimestamps(IList<IFight> pulls, DateTime startOfVideo)
        {

        }
    }
}