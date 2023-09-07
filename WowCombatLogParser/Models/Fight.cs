using System;
using System.Linq;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Models;

public class Boss : Fight<EncounterStart, EncounterEnd>
{
    private List<ICombatantInfo> _combatants;

    public Boss(EncounterStart start) : base(start)
    {
    }

    public override CombatLogEvent AddEvent(CombatLogEvent @event)
    {
        base.AddEvent(@event);

        if (@event is ICombatantInfo combatantInfoEvent)
        {
            
            Combatants.Add(combatantInfoEvent);
        }

        return @event;
    }

    public override string Name => _start.Name;
    public override string Result => _end is EncounterEnd endOfFight && endOfFight.Success ? "Kill" : "Wipe";
    public override bool IsSuccess => Result == "Kill";
    public virtual List<ICombatantInfo> Combatants { get; } = new();
}

public class Trash : IFight
{
    public Trash() { }

    protected CombatLogEvent _start;
    protected CombatLogEvent _end;
    protected List<CombatLogEvent> _events = new();

    public void Sort() => _events = _events.OrderBy(x => x.Id).ToList();
    public IList<CombatLogEvent> GetEvents() => _events;
    public virtual FightDescription GetDetails() => new(Name, Duration, _start.Timestamp, Result);
    public bool IsEndEvent(IFightEnd @event) => @event is IFightEnd;
    public TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.Zero;
    public string Name { get; set; }
    public string Result { get; } = string.Empty;
    public (long Start, long End) Range { get; set; }
    public FightDataDictionary CommonDataDictionary { get; } = new();
    public bool IsSuccess => true;
    public CombatLogEvent AddEvent(CombatLogEvent @event)
    {
        _events.Add(@event);
        @event.Encounter = this;
        return @event;
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
