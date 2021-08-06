using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Events;
using static WoWCombatLogParser.Utilities.Extensions;

namespace WoWCombatLogParser.Models
{
    public abstract class CombatLogEvent : IEventSection
    {
        private IEnumerable<string> rawData;
        private static int _count = 0;

        public CombatLogEvent()
        {
            Id = _count++;
        }

        public CombatLogEvent(string text)
        {
            rawData = Regex.Replace(text, @"\s\s", ",").Split(',');
        }

        private protected void DoParse()
        {
            var data = rawData.GetEnumerator();
            this.Parse(data).ContinueWith(result =>
            {
                data.Dispose();
                rawData = Enumerable.Empty<string>();
            });
        }

        public virtual EventBase BaseEvent { get; set; }
        [NonData]
        public int Id { get; set; }
    }

    public class CombatLogEvent<TEvent> : CombatLogEvent
        where TEvent : IEventSection, new()
    {
        public CombatLogEvent(string text) : base(text)
        {
            BaseEvent = new EventBase();
            DoParse();
        }

        public TEvent Event { get; } = new();
    }

    public class CombatLogEvent<TPrefix, TSuffix> : CombatLogEvent
        where TPrefix : IEventSection, new()
        where TSuffix : IEventSection, new()
    {
        public CombatLogEvent(string text) : base(text)
        {
            BaseEvent = new ComplexEventBase();
            DoParse();
        }

        public TPrefix Prefix { get; } = new();
        public TSuffix Suffix { get; } = new();
    }
}