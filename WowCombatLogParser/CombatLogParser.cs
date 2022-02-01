using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser
{
    public class CombatLogParser
    {
        private FileSystemWatcher _file;
        private long _lastUpdated;
        private string _filename;

        public CombatLogParser()
        {
        }

        public CombatLogParser(string filename)
        {
            Filename = filename;
        }

        public string Filename 
        {
            get { return _filename; }
            set
            {
                if (_file != null) _file.EnableRaisingEvents = false;
                _filename = value;
                if (_file != null) _file.EnableRaisingEvents = false;
            }
        }

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
            string line;
            IFight fight = null;
            while ((line = sr.ReadLine()) != null)
            {
                var @event = line.GetCombatLogEvent<CombatLogEvent>();
                if (@event != null)
                {
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
        }
    }
}
