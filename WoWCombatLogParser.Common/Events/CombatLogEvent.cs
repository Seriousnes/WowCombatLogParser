using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Common.Events
{
    public abstract class CombatLogEvent : EventSection, ICombatLogEvent
    {
        protected readonly static TextFieldReaderOptions options = new() { HasFieldsEnclosedInQuotes = true, Delimiters = new[] { ',' } };
        private static int _count = 0;
        protected ReadOnlyMemory<char> _data;

        public CombatLogEvent()
        {
            Id = ++_count;
        }

        public CombatLogEvent(string parameters, IApplicationContext context) : this()
        {            
            ApplicationContext = context;
            _data = parameters.AsMemory();
        }

        [NonData]
        public int Id { get; init; }
        public DateTime Timestamp { get; set; }
        [NonData]
        public virtual string Event => GetType().Name;
        [NonData]
        public IFight Encounter { get; set; }
        [NonData]
        public IApplicationContext ApplicationContext { get; set; }

        public async Task<bool> GetParseResultAsync()
        {
            if (_data.IsEmpty) return true;
            return await Task.Factory.StartNew(() =>
            {
                var parameters = _data.ToString();
                using var dataEnumerator = TextFieldReader.ReadFields(parameters, options).GetEnumerator();
                dataEnumerator.MoveNext();
                try
                {
                    var result = Parse(dataEnumerator, Encounter?.CommonDataDictionary, ApplicationContext.EventGenerator);
                    _data = null;
                    return result;
                }
                catch (WowCombatlogParserPropertyException ex)
                {
                    throw new WowCombatlogParserPropertyException($"Error parsing event #{Id}", ex);
                }
            });
        }

        public bool GetParseResult()
        {
            if (_data.IsEmpty) return true;
            var parameters = _data.ToString();
            using var dataEnumerator = TextFieldReader.ReadFields(parameters, options).GetEnumerator();
            dataEnumerator.MoveNext();
            try
            {
                var result = Parse(dataEnumerator, Encounter?.CommonDataDictionary, ApplicationContext.EventGenerator);
                _data = null;
                return result;
            }
            catch (WowCombatlogParserPropertyException ex)
            {
                throw new WowCombatlogParserPropertyException($"Error parsing event #{Id}", ex);
            }
        }
    }
}