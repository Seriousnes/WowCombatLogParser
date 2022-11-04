using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public abstract partial class CombatLogEvent : EventSection, ICombatLogEvent
    {
        private readonly static TextFieldReaderOptions options = new() { HasFieldsEnclosedInQuotes = true, Delimiters = new[] { ',' } };
        private string _data;
        private static int _count = 0;

        public CombatLogEvent()
        {
            Id = ++_count;
        }

        public CombatLogEvent(DateTime timestamp, string @event, string data) : this()
        {            
            Timestamp = timestamp;
            Event = @event;
            _data = data;
        }

        [NonData]
        public int Id { get; init; }
        [NonData]
        public DateTime Timestamp { get; init; }
        [NonData]
        public string Event { get; init; }
        [NonData]
        public IFight Encounter { get; set; }
        [NonData]
        public IApplicationContext ApplicationContext { get; set; }       
        
        public IList<IField> GetData(bool reset = true)
        {
            IList<IField> data = null;
            if (_data != null)
            {
                data = TextFieldReader.ReadFields(_data, options);
                if (reset) _data = null;
            }

            return data;
        }
    }
}