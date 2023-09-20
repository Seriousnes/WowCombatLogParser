using Microsoft.AspNetCore.Components;
using WoWCombatLogParser;

namespace WowCombatLogParser.App.Components;

public partial class AppComponentBase : ComponentBase
{
    [Inject]
    public IParserContext Context { get; set; }
    public ICombatLogParser CombatLogParser => Context.CombatLogParser;
    public IEventGenerator EventGenerator => Context.EventGenerator;
}
