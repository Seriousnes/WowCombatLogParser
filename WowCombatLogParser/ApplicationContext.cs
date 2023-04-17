using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser
{
    public class ApplicationContext : IApplicationContext
    {
        private ICombatLogParser combatLogParser;
        private IEventGenerator eventGenerator;
        private IMapper Mapper;

        public ApplicationContext()
        {
            CombatLogParser = new CombatLogParser();
            EventGenerator = new EventGenerator();
            Mapper = InitializeMapper();
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

        private static IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            return configuration.CreateMapper();
        }
    }
}
