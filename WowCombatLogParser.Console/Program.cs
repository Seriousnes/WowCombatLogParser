using System.Diagnostics;
using WoWCombatLogParser.Parser;

namespace WowCombatLogParser.App;

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


    }
}