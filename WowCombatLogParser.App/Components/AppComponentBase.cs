using Microsoft.AspNetCore.Components;
using WoWCombatLogParser;

namespace WowCombatLogParser.App.Components;

public partial class AppComponentBase : ComponentBase
{
    [Inject]
    public ICombatLogParser CombatLogParser { get; set; }
}
