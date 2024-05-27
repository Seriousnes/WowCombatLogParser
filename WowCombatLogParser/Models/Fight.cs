using System;
using System.Linq;

namespace WoWCombatLogParser;

public interface IFight
{
    void Sort();
    FightDescription GetDetails();
    IList<CombatLogEvent> GetEvents();
    CombatLogEvent AddEvent(CombatLogEvent combatLogEvent);
    (long Start, long End) Range { get; }
    bool IsEndEvent(CombatLogEvent type);
    string? Name { get; }
    bool IsSuccess { get; }
}

[DebuggerDisplay("{GetDetails()}")]
public abstract partial class Fight<TStart, TEnd> : IFight
    where TStart : CombatLogEvent, IFightStart
    where TEnd : CombatLogEvent, IFightEnd
{
    protected TStart _start;
    protected TEnd? _end;
    protected List<CombatLogEvent> _events = [];

    public Fight(TStart start)
    {
        _start = start;
        _events.Add(start);
    }

    public virtual CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
    {
        _events.Add(combatLogEvent);
        if (combatLogEvent is TEnd endEvent)
            _end = endEvent;
        return combatLogEvent;
    }

    public virtual void Sort() => _events = [.. _events.OrderBy(x => x.Id)];
    public IList<CombatLogEvent> GetEvents() => _events;
    public virtual FightDescription GetDetails() => new(Name, Duration, _start.Timestamp, Result);
    public virtual bool IsEndEvent(CombatLogEvent combatLogEvent) => combatLogEvent is TEnd;
    public virtual TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.FromMilliseconds(_end.Duration);
    public abstract string Name { get; }
    public abstract string Result { get; }
    public virtual (long Start, long End) Range { get; set; }
    public abstract bool IsSuccess { get; }
}

[DebuggerDisplay("{Description} ({Result}) {Duration} {Time}")]
public class FightDescription(string description, TimeSpan duration, DateTime time, string result)
{
    public string Description { get; set; } = description;
    public string Duration { get; set; } = $"{duration:m\\:ss}";
    public string Result { get; set; } = result;
    public DateTime Time { get; set; } = time;
    public override string ToString()
    {
        return $"{Description} ({Result}) {Duration} {Time:h:mm tt}";
    }
}


public class Boss(EncounterStart start) : Fight<EncounterStart, EncounterEnd>(start)
{
    public override CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
    {
        base.AddEvent(combatLogEvent);

        if (combatLogEvent is ICombatantInfo combatantInfoEvent)
        {
            
            Combatants.Add(combatantInfoEvent);
        }

        return combatLogEvent;
    }

    public override string Name => _start.Name ?? "Unknown";
    public override string Result => _end is EncounterEnd endOfFight && endOfFight.Success ? "Kill" : "Wipe";
    public override bool IsSuccess => Result == "Kill";
    public virtual List<ICombatantInfo> Combatants { get; } = [];
}

public class Trash : IFight
{
    public Trash(CombatLogEvent start) 
    {
        _events.Add(_start = start);
    }

    protected CombatLogEvent _start;
    protected CombatLogEvent? _end;
    protected List<CombatLogEvent> _events = [];

    public void Sort() => _events = [.. _events.OrderBy(x => x.Id)];
    public IList<CombatLogEvent> GetEvents() => _events;
    public virtual FightDescription GetDetails() => new(Name ?? "Unknown", Duration, _start.Timestamp, Result);
    public bool IsEndEvent(CombatLogEvent combatLogEvent) => combatLogEvent is IFightEnd;
    public TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.Zero;
    public string? Name { get; set; }
    public string Result { get; } = string.Empty;
    public (long Start, long End) Range { get; set; }
    public bool IsSuccess => true;
    public CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
    {
        _events.Add(combatLogEvent);
        return combatLogEvent;
    }
}

public class ChallengeMode(ChallengeModeStart start) : Fight<ChallengeModeStart, ChallengeModeEnd>(start)
{
    public override string Name => $"{_start.InstanceId} Level {_start.KeystoneLevel} (Affixes: {string.Join(',', _start.Affixes?.Select(x => x.Id.ToString()) ?? [])})";
    public override string Result => _end is ChallengeModeEnd endOfFight && endOfFight.Success ? "Timed" : "Not timed";
    public override bool IsSuccess => Result == "Timed";

}

public class ArenaMatch(ArenaMatchStart start) : Fight<ArenaMatchStart, ArenaMatchEnd>(start)
{
    public override string Name => _start.InstanceId.ToString();
    public override string Result => _end is ArenaMatchEnd endOfFight ? $"Team {endOfFight.WinningTeam} wins. New ratings: Team1 = {endOfFight.NewRatingTeam1}, Team2 = {endOfFight.NewRatingTeam2}" : "";
    public override bool IsSuccess => Result.Contains("wins", StringComparison.InvariantCultureIgnoreCase);
}
