using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Models
{
    public abstract class CombatLogEvent : EventSection, ICombatLogEvent
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
        
        public void Parse()
        {
            if (_data != null)
            {
                var data = TextFieldReader.ReadFields(_data, options)?.GetEnumerator();
                if (data?.MoveNext() ?? false)
                {
                    Parse(data);
                }
                data?.Dispose();
                _data = null;
            }
        }

        public async Task<ICombatLogEvent> ParseAsync()
        {
            await Task.Run(() => Parse());
            return this;
        }
    }
}