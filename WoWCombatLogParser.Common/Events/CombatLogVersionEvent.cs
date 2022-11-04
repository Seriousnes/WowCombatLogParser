using System;
using System.Collections.Generic;
using System.Reflection;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Common.Events
{
    [Affix("COMBAT_LOG_VERSION")]
    public class CombatLogVersionEvent : CombatLogEvent
    {
        public CombatLogVersionEvent() : base() { }

        public CombatLogVersionEvent(DateTime timestamp, string @event, string data) : base(timestamp, @event, data)
        {
        }

        public CombatLogVersion Version { get; set; }
        [Offset(1)]
        public bool AdvancedLogEnabled { get; set; }
        [Offset(1)]
        public string BuildVersion { get; set; }
        [Offset(1)]
        public int ProjectId { get; set; }

        public override bool Parse(IEventGenerator eventGenerator, FightDataDictionary fightDataDictionary, IEnumerator<IField> data)
        {
            foreach (var property in eventGenerator.GetClassMap(this.GetType()).Properties)
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
