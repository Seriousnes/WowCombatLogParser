using Microsoft.AspNetCore.Components;
using WoWCombatLogParser;
using WoWCombatLogParser.Parser;

namespace WowCombatLogParser.App.Components;

public partial class AppComponentBase : ComponentBase
{
    [Inject]
    public IApplicationContext Context { get; set; }
    public ICombatLogParser CombatLogParser => Context.CombatLogParser;
    public IEventGenerator EventGenerator => Context.EventGenerator;
}
