using System;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser
{
    public class CombatLogParser : ICombatLogParser
    {
        private FileSystemWatcher _file;
        private long _lastUpdated;
        private string _filename;

        public CombatLogParser()
        {
        }

        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_file != null) _file.EnableRaisingEvents = false;
                _filename = value;
                if (_file != null) _file.EnableRaisingEvents = true;
            }
        }

        public IApplicationContext ApplicationContext { get; set; }

        public void Watch(params FileSystemEventHandler[] fileChanged)
        {
            if (_file != null)
                throw new InvalidOperationException($"CombatLogParser is already attached to {_file.Path}. Call Detatch() before attaching to a file.");

            _file = new FileSystemWatcher(_filename) { EnableRaisingEvents = false, NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime };
            fileChanged.ToList().ForEach(e =>
            {
                _file.Changed += e;
                _file.Created += e;
            });
            _file.EnableRaisingEvents = true;
            _lastUpdated = DateTime.Now.Ticks;
        }

        public void Stop()
        {
            _file.Dispose();
            _file = null;
        }

        public IEnumerable<IFight> Scan()
        {
            if (string.IsNullOrEmpty(_filename))
                throw new ArgumentNullException("Filename", "Filename must not be null");
            if (!File.Exists(_filename))
                yield break;

            using var sr = new StreamReader(_filename);
            SetupEventGenerator(sr);
            string line;
            IFight fight = null;
            while ((line = sr.ReadLine()) != null)
            {
                var @event = ApplicationContext.EventGenerator.GetCombatLogEvent<CombatLogEvent>(line);
                if (@event is null) continue;
                switch (@event)
                {
                    case IFightEnd end when @event is IFightEnd:
                        if (fight != null && fight.IsEndEvent(end))
                            fight.AddEvent(@event);
                        yield return fight;
                        fight = null;
                        break;
                    case IFightStart start when @event is IFightStart:
                        fight = start.GetFight();
                        break;
                    default:
                        fight?.AddEvent(@event);
                        break;
                }
            }
        }

        public void Parse(ICombatLogEvent combatLogEvent, IFight encounter = null)
        {
            var dataEnumerator = combatLogEvent.GetData()?.GetEnumerator();
            if (dataEnumerator?.MoveNext() ?? false)
            {
                combatLogEvent.Parse(ApplicationContext.EventGenerator, encounter?.CommonDataDictionary, dataEnumerator);
            }
            
            dataEnumerator?.Dispose();
        }

        public void Parse(IFight encounter)
        {
            foreach (var combatLogEvent in encounter.GetEvents())
            {
                Parse(combatLogEvent, encounter);
            }
        }

        public void Parse(IEnumerable<IFight> encounters)
        {
            foreach (var encounter in encounters)
                Parse(encounter);
        }

        public async Task ParseAsync(ICombatLogEvent combatLogEvent, IFight encounter = null) => 
            await Task.Run(() => Parse(combatLogEvent, encounter));

        public async Task ParseAsync(IFight encounter) => 
            await Parallel.ForEachAsync(
                encounter.GetEvents(),                
                async (c, _) => await ParseAsync(c, encounter));

        public async Task ParseAsync(IEnumerable<IFight> encounters)
        {
            await Parallel.ForEachAsync(encounters, async (e, _) => await ParseAsync(e));
        }

        private void SetupEventGenerator(StreamReader sr)
        {
            var currentPosition = sr.BaseStream.Position;
            sr.BaseStream.Position = 0;

            string line = sr.ReadLine();
            if (line != null)
            {
                ApplicationContext.EventGenerator = new EventGenerator() { ApplicationContext = ApplicationContext };
                ApplicationContext.EventGenerator.SetCombatLogVersion(line);
            }

            sr.BaseStream.Position = currentPosition;
        }
    }
}
