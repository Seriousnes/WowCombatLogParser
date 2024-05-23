using System;
using System.Linq;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Models;

public interface IFight
{
    void Sort();
    FightDescription GetDetails();
    IList<CombatLogEvent> GetEvents();
    CombatLogEvent AddEvent(CombatLogEvent combatLogEvent);
    (long Start, long End) Range { get; }
    bool IsEndEvent(IFightEnd type);
    string Name { get; }
    bool IsSuccess { get; }
}

[DebuggerDisplay("{GetDetails()}")]
public abstract partial class Fight<TStart, TEnd> : IFight
    where TStart : CombatLogEvent, IFightStart
    where TEnd : CombatLogEvent, IFightEnd
{
    protected TStart _start;
    protected TEnd _end;
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
    public virtual bool IsEndEvent(IFightEnd @CombatLogEventComponent) => typeof(TEnd).IsAssignableFrom(@CombatLogEventComponent.GetType());
    public virtual TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.FromMilliseconds(_end.Duration);
    public abstract string Name { get; }
    public abstract string Result { get; }
    public virtual (long Start, long End) Range { get; set; }
    public abstract bool IsSuccess { get; }
}

[DebuggerDisplay("{Description} ({Result}) {Duration} {Time}")]
public class FightDescription
{
    public FightDescription(string description, TimeSpan duration, DateTime time, string result)
    {
        Description = description;
        Duration = $"{duration:m\\:ss}";
        Time = time;
        Result = result;
    }

    public string Description { get; set; }
    public string Duration { get; set; }
    public string Result { get; set; }
    public DateTime Time { get; set; }
    public override string ToString()
    {
        return $"{Description} ({Result}) {Duration} {Time:h:mm tt}";
    }
}


public class Boss : Fight<EncounterStart, EncounterEnd>
{
    private List<ICombatantInfo> _combatants;

    public Boss(EncounterStart start) : base(start)
    {
    }

    public override CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
    {
        base.AddEvent(combatLogEvent);

        if (combatLogEvent is ICombatantInfo combatantInfoEvent)
        {
            
            Combatants.Add(combatantInfoEvent);
        }

        return combatLogEvent;
    }

    public override string Name => _start.Name;
    public override string Result => _end is EncounterEnd endOfFight && endOfFight.Success ? "Kill" : "Wipe";
    public override bool IsSuccess => Result == "Kill";
    public virtual List<ICombatantInfo> Combatants { get; } = [];
}

public class Trash : IFight
{
    public Trash() { }

    protected CombatLogEvent _start;
    protected CombatLogEvent _end;
    protected List<CombatLogEvent> _events = [];

    public void Sort() => _events = [.. _events.OrderBy(x => x.Id)];
    public IList<CombatLogEvent> GetEvents() => _events;
    public virtual FightDescription GetDetails() => new(Name, Duration, _start.Timestamp, Result);
    public bool IsEndEvent(IFightEnd @CombatLogEventComponent) => @CombatLogEventComponent is IFightEnd;
    public TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.Zero;
    public string Name { get; set; }
    public string Result { get; } = string.Empty;
    public (long Start, long End) Range { get; set; }
    public bool IsSuccess => true;
    public CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
    {
        _events.Add(combatLogEvent);
        return combatLogEvent;
    }
}

public class ChallengeMode : Fight<ChallengeModeStart, ChallengeModeEnd>
{
    public ChallengeMode(ChallengeModeStart start) : base(start)
    {
    }

    public override string Name => $"{_start.InstanceId} Level {_start.KeystoneLevel} (Affixes: {string.Join(',', _start.Affixes?.Select(x => x.Id.ToString()))})";
    public override string Result => _end is ChallengeModeEnd endOfFight && endOfFight.Success ? "Timed" : "Not timed";
    public override bool IsSuccess => Result == "Timed";

}

public class ArenaMatch : Fight<ArenaMatchStart, ArenaMatchEnd>
{
    public ArenaMatch(ArenaMatchStart start) : base(start)
    {
    }

    public override string Name => _start.InstanceId.ToString();
    public override string Result => _end is ArenaMatchEnd endOfFight ? $"Team {endOfFight.WinningTeam} wins. New ratings: Team1 = {endOfFight.NewRatingTeam1}, Team2 = {endOfFight.NewRatingTeam2}" : "";
    public override bool IsSuccess => Result.Contains("wins", StringComparison.InvariantCultureIgnoreCase);
}
