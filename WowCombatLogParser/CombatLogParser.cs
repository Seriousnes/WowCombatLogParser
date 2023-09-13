using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser;

public interface ICombatLogParser
{
    IApplicationContext ApplicationContext { get; set; }
    string Filename { get; set; }
    IEnumerable<IFight> Scan(bool quickScan = false);
    void Stop();
    void Watch(params FileSystemEventHandler[] fileChanged);
}

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

    public IEnumerable<IFight> Scan(bool quickScan = false)
    {
        if (string.IsNullOrEmpty(_filename))
            throw new ArgumentNullException("Filename", "Filename must not be null");
        if (!File.Exists(_filename))
            yield break;

        using var cr = new CombatLogStreamReader(_filename, ApplicationContext);
        IFight? fight = null;
        foreach (var line in cr.ReadLines())
        {
            var combatLogEvent = ApplicationContext.EventGenerator.GetCombatLogEvent<CombatLogEvent>(line);
            switch (combatLogEvent)
            {
                case IFightEnd end when combatLogEvent is IFightEnd:
                    if (fight is { })
                    {
                        if (fight.IsEndEvent(end))
                            fight.AddEvent(combatLogEvent);
                        yield return fight;
                    }                                        
                    fight = null;
                    break;
                case IFightStart start when combatLogEvent is IFightStart:
                    fight = start.GetFight();
                    break;
                case null:
                    break;
                default:
                    fight?.AddEvent(combatLogEvent);
                    break;
            }
        }
    }
}
