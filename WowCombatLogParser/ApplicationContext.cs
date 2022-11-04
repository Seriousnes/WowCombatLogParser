using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser
{
    public class ApplicationContext : IApplicationContext
    {
        private ICombatLogParser combatLogParser;
        private IEventGenerator eventGenerator;

        public ApplicationContext()
        {
            CombatLogParser = new CombatLogParser();
            EventGenerator = new EventGenerator();
        }

        public ApplicationContext(ICombatLogParser combatLogParser, IEventGenerator eventGenerator)
        {
            CombatLogParser = combatLogParser;
            EventGenerator = eventGenerator;
        }

        public ICombatLogParser CombatLogParser 
        { 
            get => combatLogParser;
            set 
            { 
                combatLogParser = value;
                combatLogParser.ApplicationContext = this;
            }
        }
        public IEventGenerator EventGenerator
        {
            get => eventGenerator;
            set
            {
                eventGenerator = value;
                eventGenerator.ApplicationContext = this;
            }
        }
    }
}
