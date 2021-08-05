using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;

namespace WoWCombatLogParser.Models
{
    public abstract class CombatLogEvent : EventSection
    {
        private readonly IEnumerable<string> rawData;
        public CombatLogEvent(string text)
        {
            rawData = Regex.Replace(text, @"\s\s", ",").Split(',').ToList();
        }
        public virtual EventBase BaseEvent { get; set; }
        private protected IEnumerator<string> DataEnumerator => rawData.GetEnumerator();
    }

    public class CombatLogEvent<TEvent> : CombatLogEvent
        where TEvent : EventSection, new()
    {
        public CombatLogEvent(string text) : base(text)
        {
            BaseEvent = new EventBase();
            Parse(DataEnumerator);
        }

        public TEvent Event { get; } = new TEvent();
    }

    public class CombatLogEvent<TPrefix, TSuffix> : CombatLogEvent
        where TPrefix : EventSection, new()
        where TSuffix : EventSection, new()
    {
        public CombatLogEvent(string text) : base(text)
        {
            BaseEvent = new ComplexEventBase();
            Parse(DataEnumerator);
        }

        public TPrefix Prefix { get; } = new TPrefix();
        public TSuffix Suffix { get; } = new TSuffix();
    }      
}
