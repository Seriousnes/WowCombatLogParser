using AutoMapper;
using System.Reflection;

namespace WoWCombatLogParser;

public interface IParserContext
{
    ICombatLogParser CombatLogParser { get; set; }
    IEventGenerator EventGenerator { get; set; }
    ICombatLogEventMapper CombatLogEventMapper { get; set; }
}

public class ParserContext : IParserContext
{
    private ICombatLogParser combatLogParser;
    private IEventGenerator eventGenerator;
    private ICombatLogEventMapper combatLogEventMapper;
    private IMapper Mapper;

    public ParserContext() : this(new CombatLogParser(), new EventGenerator(), new CombatLogEventMapper())
    {
        Mapper = InitializeMapper();
    }

    public ParserContext(ICombatLogParser combatLogParser, IEventGenerator eventGenerator, ICombatLogEventMapper combatLogEventMapper)
    {
        CombatLogParser = combatLogParser;
        EventGenerator = eventGenerator;
        CombatLogEventMapper = combatLogEventMapper;
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

    public ICombatLogEventMapper CombatLogEventMapper
    {
        get => combatLogEventMapper;
        set => combatLogEventMapper = value;
    }

    private static IMapper InitializeMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        return configuration.CreateMapper();
    }
}
