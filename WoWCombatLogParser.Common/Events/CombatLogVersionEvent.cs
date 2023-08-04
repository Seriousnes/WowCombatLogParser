using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Common.Events
{
    [Affix("COMBAT_LOG_VERSION")]
    public class CombatLogVersionEvent : CombatLogEvent
    {
        public CombatLogVersionEvent() : base() { }

        public CombatLogVersionEvent(string parameters, IApplicationContext context) : base(parameters, context)
        {
            GetParseResultAsync().Wait();
        }

        public CombatLogVersion Version { get; set; }
        [Offset(1)]
        public bool AdvancedLogEnabled { get; set; }
        [Offset(1)]
        public string BuildVersion { get; set; }
        [Offset(1)]
        public int ProjectId { get; set; }
        
        internal override bool Parse(IEnumerator<IField> data, FightDataDictionary fightDataDictionary = null, IEventGenerator eventGenerator = null)
        {
            eventGenerator ??= ApplicationContext.EventGenerator;
            foreach (var property in this.GetType().GetTypePropertyInfo())
            {
                var steps = property.GetCustomAttribute<OffsetAttribute>()?.Value ?? 0;
                data.MoveBy(steps);
                property.SetValue(this, Conversion.GetValue(data.Current, property.PropertyType));
                data.MoveNext();
            }
            return true;
        }
    }
}
