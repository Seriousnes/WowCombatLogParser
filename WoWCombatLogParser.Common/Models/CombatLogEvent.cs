using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models
{
    public abstract partial class CombatLogEvent : EventSection, ICombatLogEvent
    {
        private readonly static TextFieldReaderOptions options = new() { HasFieldsEnclosedInQuotes = true, Delimiters = new[] { ',' } };
        private string _data;
        private static int _count = 0;
        protected CombatLogVersion[] CombatLogVersions;

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
        
        public void Parse()
        {
            if (_data != null)
            {
                var data = TextFieldReader.ReadFields(_data, options)?.GetEnumerator();
                if (data?.MoveNext() ?? false)
                {
                    Parse(Encounter?.CommonDataDictionary, data);
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

        public bool IsApplicableForVersion(CombatLogVersion version)
        {
            return CombatLogVersions == null || CombatLogVersions.Length == 0 || CombatLogVersions.Contains(version);
        }
    }
}