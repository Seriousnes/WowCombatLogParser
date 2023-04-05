using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WowCombatLogParser.App.Components
{
    public partial class AppComponentBase : ComponentBase
    {
        [Inject]
        public IApplicationContext Context { get; set; }

        public ICombatLogParser CombatLogParser => Context.CombatLogParser;
        public IEventGenerator EventGenerator => Context.EventGenerator;
    }
}
