using System;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

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

        public CombatLogEvent(IList<IField> line) : this(line, false)
        {
        }

        public CombatLogEvent(IList<IField> line = null, bool parseImmediate = false) : this()
        {
            _line = line;            
        }

        public DateTime Timestamp { get; set; }
        public string Event { get; set; }

        [NonData]
        public int Id { get; init; }
        [NonData]
        public bool HasBeenParsed { get; private set; } = false;        
        
        public void Parse()
        {
            if (HasBeenParsed) return;
            HasBeenParsed = true;
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