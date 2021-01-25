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
        public CombatLogParser()
        {
            //System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(EventGenerator).TypeHandle);
        }

        public void Parse(string fileName)
        {
            Parse(File.ReadAllLines(fileName));
        }

        public void Parse(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var @event = EventGenerator.GetCombatLogEvent(line);
                if (@event != null)
                {
                    Events.Add(@event);
                }
            }
        }

        public List<CombatLogEvent> Events { get; } = new List<CombatLogEvent>();
    }
}
