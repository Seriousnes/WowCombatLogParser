using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;

namespace WoWCombatLogParser.Models
{
    public abstract class CombatLogEvent : EventSection
    {
        private readonly IList<string> rawData;

        public CombatLogEvent(string text)
        {
            rawData = Regex.Replace(text, @"\s\s", ",").Split(',').ToList();            
        }

        private protected void DoParse()
        {
            using var enumerator = rawData.GetEnumerator();
            try
            {
                Parse(enumerator);
            }
            catch (CombatLogParseException ex)
            {
                Console.WriteLine($"{ex} Current line: {string.Join(',', rawData)}");
            }            
            catch
            {
            }
            rawData.Clear();
        }

        public virtual EventBase BaseEvent { get; set; }
    }

    public class CombatLogEvent<TEvent> : CombatLogEvent
        where TEvent : EventSection, new()
    {
        public CombatLogEvent(string text) : base(text)
        {
            BaseEvent = new EventBase();
            DoParse();
        }

        public TEvent Event { get; } = new();
    }

    public class CombatLogEvent<TPrefix, TSuffix> : CombatLogEvent
        where TPrefix : EventSection, new()
        where TSuffix : EventSection, new()
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