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
        private IEnumerable<IField> _line;
        private static int _count = 0;

        public CombatLogEvent()
        {
            Id = ++_count;
        }

        public CombatLogEvent(IList<IField> line) : this()
        {            
            Timestamp = DateTime.ParseExact(line[(int)FieldIndex.Timestamp].ToString(), "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Event = line[(int)FieldIndex.EventType].ToString();
            _line = line.Skip(2).ToList();
        }

        [NonData]
        public int Id { get; init; }
        [NonData]
        public DateTime Timestamp { get; init; }
        [NonData]
        public string Event { get; init; }
        
        public void Parse()
        {
            var data = _line?.GetEnumerator();
            if (data?.MoveNext() ?? false)
            {
                Parse(data);
            }
            data?.Dispose();
            _line = null;
        }

        public async Task<ICombatLogEvent> ParseAsync()
        {
            await Task.Run(() => Parse());
            return this;
        }
    }
}