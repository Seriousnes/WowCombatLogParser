using System.Threading.Tasks;

namespace WoWCombatLogParser.Models;

internal interface IParsable
{
    Task Parse(List<ICombatLogDataField> data);
    Task Parse(string line);
    IParserContext ParserContext { get; set; }
}
