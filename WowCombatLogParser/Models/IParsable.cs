using System.Threading.Tasks;

namespace WoWCombatLogParser.Models;

internal interface IParsable
{
    Task Parse(IList<ICombatLogDataField> data);
    Task Parse(string line);
    IParserContext ParserContext { get; set; }
}
