using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser;

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
        IFight fight = null;
        foreach (var line in cr.ReadLines())
        {
            var @CombagLogEventComponent = ApplicationContext.EventGenerator.GetCombatLogEvent<CombatLogEvent>(line);
            switch (@CombagLogEventComponent)
            {
                case IFightEnd end when @CombagLogEventComponent is IFightEnd:
                    if (fight != null && fight.IsEndEvent(end))
                    {
                        fight.AddEvent(@CombagLogEventComponent);
                    }                    
                    yield return fight;
                    fight = null;
                    break;
                case IFightStart start when @CombagLogEventComponent is IFightStart:
                    ParseAsync(@CombagLogEventComponent).Wait();
                    fight = start.GetFight();
                    break;
                case null:
                    break;
                default:
                    fight?.AddEvent(@CombagLogEventComponent);
                    break;
            }
        }
    }

    public void Parse(IFight encounter)
    {
        foreach (var combatLogEvent in encounter.GetEvents())
        {
        }
    }

    public void Parse(IEnumerable<IFight> encounters)
    {
        foreach (var encounter in encounters)
            Parse(encounter);
    }

    public async Task ParseAsync(ICombatLogEvent combatLogEvent, IFight encounter = null) => 
        await Task.Run(() => { });

    public async Task ParseAsync(IFight encounter) => 
        await Parallel.ForEachAsync(
            encounter.GetEvents(),                
            async (c, _) => await ParseAsync(c, encounter));

    public async Task ParseAsync(IEnumerable<IFight> encounters)
    {
        await Parallel.ForEachAsync(encounters, async (e, _) => await ParseAsync(e));
    }

    private void SetupEventGenerator(StreamReader sr, bool resetToCurrentPosition = true)
    {
        var currentPosition = sr.BaseStream.Position;
        sr.BaseStream.Position = 0;

        string line = sr.ReadLine();
        if (line != null)
        {
            ApplicationContext.EventGenerator = new EventGenerator() { ApplicationContext = ApplicationContext };
            ApplicationContext.EventGenerator.SetCombatLogVersion(line);
        }

        if (resetToCurrentPosition)
            sr.BaseStream.Position = currentPosition;
    }
}
