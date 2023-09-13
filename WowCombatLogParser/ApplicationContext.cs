using AutoMapper;
using System.Reflection;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser;

public interface IApplicationContext
{
    ICombatLogParser CombatLogParser { get; set; }
    IEventGenerator EventGenerator { get; set; }
}

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
