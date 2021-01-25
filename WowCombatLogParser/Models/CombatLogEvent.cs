using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;

namespace WoWCombatLogParser.Models
{
    public class CombatLogEvent : EventSection
    {
        public CombatLogEvent(string[] args)
        {
            using var _enumerator = args.AsEnumerable().GetEnumerator();            
            Parse(_enumerator);            
        }           
    }

    public class CombatLogEvent<TEvent> : CombatLogEvent
        where TEvent : EventSection, new()
    {
        public CombatLogEvent(string[] args) : base(args)
        {
        }

        [FieldOrder(1)]
        public SimpleEventBase BaseEvent { get; } = new SimpleEventBase();
        [FieldOrder(2)]
        public TEvent Event { get; } = new TEvent();
    }

    public class CombatLogEvent<TPrefix, TSuffix> : CombatLogEvent
        where TPrefix : EventSection, new()
        where TSuffix : EventSection, new()
    {
        public CombatLogEvent(string[] args) : base(args)
        {
        }

        [FieldOrder(1)]
        public ComplexEventBase BaseEvent { get; } = new ComplexEventBase();
        [FieldOrder(2)]
        public TPrefix Prefix { get; } = new TPrefix();
        [FieldOrder(3)]
        public TSuffix Suffix { get; } = new TSuffix();
    }      
}
