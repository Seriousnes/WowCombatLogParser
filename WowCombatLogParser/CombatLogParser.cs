using WoWCombatLogParser.Parser;

namespace WoWCombatLogParser;

public interface ICombatLogParser
{
    IApplicationContext ApplicationContext { get; set; }

    CombatLogEvent ParseLine(string text);
}

public class CombatLogParser : ICombatLogParser
{
    public IApplicationContext ApplicationContext { get; set; }
    public CombatLogEvent ParseLine(string line)
    {
        return ApplicationContext.EventGenerator.GetCombatLogEvent<CombatLogEvent>(line);
    }
}
