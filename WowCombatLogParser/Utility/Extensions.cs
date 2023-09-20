using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Utility;

public enum IndexMode
{
    LineStart,
    LineEnd,
}

public static class Extensions
{
    public static (bool Success, IEnumerator<ICombatLogDataField> Enumerator, bool EndOfParent, bool Dispose) GetEnumeratorForProperty(this IEnumerator<ICombatLogDataField> data)
    {
        if (data.Current is CombatLogDataFieldCollection groupData)
        {
            var enumerator = groupData.Children.GetEnumerator();
            return (enumerator.MoveNext(), enumerator, !data.MoveNext(), true);
        }

        return (true, data, false, false);
    }

    public static void Forget(this Task _)
    {
    }

    public static int IndexOfAny(this string _string, IndexMode mode, int startIndex, params string[] values)
    {
        IList<int> results = Array.Empty<int>();
        foreach (var value in values)
        {
            var index = _string.IndexOf(value, results.Any() ? results.Min() : startIndex, StringComparison.Ordinal);
            if (index >= 0)
            {
                var indexOfLinebreak = mode switch
                {
                    IndexMode.LineStart => _string.LastIndexOf(Environment.NewLine, index, StringComparison.Ordinal) + Environment.NewLine.Length,
                    IndexMode.LineEnd => _string.IndexOf(Environment.NewLine, index, StringComparison.Ordinal) + Environment.NewLine.Length,
                    _ => -1
                };
                if (indexOfLinebreak >= 0)
                    results.Add(indexOfLinebreak);
            }            
        }
        return results.Any() ? results.Min() : -1;
    }

    public static int IndexOf(this string _string, string value, int startIndex, IndexMode mode)
    {
        var index = _string.IndexOf(value, startIndex, StringComparison.Ordinal);
        if (index >= 0)
        {
            return mode switch
            {
                IndexMode.LineStart => _string.LastIndexOf(Environment.NewLine, index - 1, StringComparison.Ordinal) + Environment.NewLine.Length,
                IndexMode.LineEnd => _string.IndexOf(Environment.NewLine, index + 1, StringComparison.Ordinal)
            };
        }
        return -1;
    }

    public static IFight? GetFight(this IFightStart start)
    {
        return start switch
        {
            EncounterStart raidStart when start.GetType() == typeof(EncounterStart) => new Boss(raidStart),
            ArenaMatchStart arenaStart when start.GetType() == typeof(ArenaMatchStart) => new ArenaMatch(arenaStart),
            ChallengeModeStart challengeStart when start.GetType() == typeof(ChallengeModeStart) => new ChallengeMode(challengeStart),
            _ => null
        };
    }

    public static bool Implements<T>(this Type type)
    {        
        return typeof(T).IsAssignableFrom(type);
    }

    public static DateTime GetTimestamp(this string dateTimeString) => DateTime.TryParseExact(dateTimeString, "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var timestamp) ? timestamp : DateTime.MinValue;

    public static DifficultyInfo GetDifficultyInfo(this Difficulty difficulty) => difficulty switch
    {
        Difficulty.Normal => new DifficultyInfo { Name = "Normal", Type = InstanceType.Party },
        Difficulty.Heroic => new DifficultyInfo { Name = "Heroic", Type = InstanceType.Party },
        Difficulty.Player10 => new DifficultyInfo { Name = "10 Player", Type = InstanceType.Raid },
        Difficulty.Player25 => new DifficultyInfo { Name = "25 Player", Type = InstanceType.Raid },
        Difficulty.Player10Heroic => new DifficultyInfo { Name = "10 Player (Heroic)", Type = InstanceType.Raid },
        Difficulty.Player25Heroic => new DifficultyInfo { Name = "25 Player (Heroic)", Type = InstanceType.Raid },
        Difficulty.LookingForRaid_Legacy => new DifficultyInfo { Name = "Looking For Raid (Legacy)", Type = InstanceType.Raid },
        Difficulty.MythicKeystone => new DifficultyInfo { Name = "Mythic Keystone", Type = InstanceType.Party },
        Difficulty.Player40 => new DifficultyInfo { Name = "40 Player", Type = InstanceType.Raid },
        Difficulty.HeroicScenario => new DifficultyInfo { Name = "Heroic Scenario", Type = InstanceType.Scenario },
        Difficulty.NormalScenario => new DifficultyInfo { Name = "Normal Scenario", Type = InstanceType.Scenario },
        Difficulty.NormalRaid => new DifficultyInfo { Name = "Normal", Type = InstanceType.Raid },
        Difficulty.HeroicRaid => new DifficultyInfo { Name = "Heroic", Type = InstanceType.Raid },
        Difficulty.MythicRaid => new DifficultyInfo { Name = "Mythic", Type = InstanceType.Raid },
        Difficulty.LookingForRaid => new DifficultyInfo { Name = "Looking For Raid", Type = InstanceType.Raid },
        Difficulty.Event_Raid => new DifficultyInfo { Name = "CombatLogEventComponent", Type = InstanceType.Raid },
        Difficulty.Event_Party => new DifficultyInfo { Name = "CombatLogEventComponent", Type = InstanceType.Party },
        Difficulty.EventScenario_Scenario => new DifficultyInfo { Name = "CombatLogEventComponent Scenario", Type = InstanceType.Scenario },
        Difficulty.MythicDungeon => new DifficultyInfo { Name = "Mythic", Type = InstanceType.Party },
        Difficulty.WorldPvPScenario => new DifficultyInfo { Name = "World PvP Scenario", Type = InstanceType.Party },
        Difficulty.PvEvPScenario => new DifficultyInfo { Name = "PvEvP Scenario", Type = InstanceType.Scenario },
        Difficulty.EventScenario => new DifficultyInfo { Name = "CombatLogEventComponent", Type = InstanceType.Scenario },
        Difficulty.WorldPvPScenario1 => new DifficultyInfo { Name = "World PvP Scenario", Type = InstanceType.Scenario },
        Difficulty.TimewalkingRaid => new DifficultyInfo { Name = "Timewalking", Type = InstanceType.Scenario },
        Difficulty.PvP => new DifficultyInfo { Name = "PvP", Type = InstanceType.Scenario },
        Difficulty.Normal_Scenario => new DifficultyInfo { Name = "Normal", Type = InstanceType.Scenario },
        Difficulty.Heroic_Scenario => new DifficultyInfo { Name = "Heroic", Type = InstanceType.Scenario },
        Difficulty.Mythic_Scenario => new DifficultyInfo { Name = "Mythic", Type = InstanceType.Scenario },
        Difficulty.PvP_Scenario => new DifficultyInfo { Name = "PvP", Type = InstanceType.Scenario },
        Difficulty.Normal_Scenario_Warfronts => new DifficultyInfo { Name = "Normal", Type = InstanceType.Scenario },
        Difficulty.Heroic_Scenario_Warfronts => new DifficultyInfo { Name = "Heroic", Type = InstanceType.Scenario },
        Difficulty.Normal_Party => new DifficultyInfo { Name = "Normal", Type = InstanceType.Party },
        Difficulty.LFR_Timewalking => new DifficultyInfo { Name = "Looking For Raid (Timewalking)", Type = InstanceType.Raid },
        Difficulty.VisionsOfNZoth => new DifficultyInfo { Name = @"Visions of N'Zoth", Type = InstanceType.Scenario },
        Difficulty.TeemingIsland => new DifficultyInfo { Name = "Teeming Island", Type = InstanceType.Scenario },
        Difficulty.Torghast => new DifficultyInfo { Name = "Torghast", Type = InstanceType.Scenario },
        Difficulty.PathOfAscension_Courage => new DifficultyInfo { Name = "PathOfAscension: Courage", Type = InstanceType.Scenario },
        Difficulty.PathOfAscension_Loyalty => new DifficultyInfo { Name = "PathOfAscension: Loyalty", Type = InstanceType.Scenario },
        Difficulty.PathOfAscension_Wisdom => new DifficultyInfo { Name = "PathOfAscension: Wisdom", Type = InstanceType.Scenario },
        Difficulty.PathOfAscension_Humility => new DifficultyInfo { Name = "PathOfAscension: Humility", Type = InstanceType.Scenario },
        Difficulty.WorldBoss => new DifficultyInfo { Name = "World Boss", Type = InstanceType.None },
        _ => throw new ArgumentException($"{difficulty} is not a valid enum value for Difficulty", nameof(difficulty))
    };
}
