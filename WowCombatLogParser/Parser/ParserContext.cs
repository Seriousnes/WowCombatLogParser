using AutoMapper;
using System.Reflection;

namespace WoWCombatLogParser;

public interface IParserContext
{
    ICombatLogParser CombatLogParser { get; set; }
    IEventGenerator EventGenerator { get; set; }
}

public class ParserContext : IParserContext
{
    private ICombatLogParser combatLogParser;
    private IEventGenerator eventGenerator;
    private IMapper Mapper;

    public ParserContext() : this(new CombatLogParser(), new EventGenerator())
    {
        Mapper = InitializeMapper();
    }

    public ParserContext(ICombatLogParser combatLogParser, IEventGenerator eventGenerator)
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
            combatLogParser.ParserContext = this;
        }
    }

    public IEventGenerator EventGenerator
    {
        get => eventGenerator;
        set
        {
            eventGenerator = value;
            eventGenerator.ParserContext = this;
        }
    }

    private static IMapper InitializeMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        return configuration.CreateMapper();
    }
}
