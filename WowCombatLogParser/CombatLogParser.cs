using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser
{
    public class CombatLogParser
    {
        public List<CombatLogEvent> Events { get; } = new List<CombatLogEvent>();

        public void Parse(string fileName)
        {
            Parse(File.ReadAllLinesAsync(fileName).Result);
        }

        public void Parse(IEnumerable<string> lines)
        {
            Parallel.ForEach(lines, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, (line) =>
            {
                Events.AddRange(EventGenerator.GetCombatLogEvent(line));
                //var @event = ;
                //if (@event != null)
                //{
                //    Events.Add(@event);
                //}
            });
        }

        private void OptimizeEvent(CombatLogEvent @event)
        {

        }
    }
}
