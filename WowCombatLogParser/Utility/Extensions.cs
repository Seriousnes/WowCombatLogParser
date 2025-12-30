using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Utility;

public enum IndexMode
{
    LineStart,
    LineEnd,
}

public static class Extensions
{
    public static bool MoveBy<T>(this IEnumerator<T> enumerator, int steps = 1)
    {
        var moveResult = true;
        while (moveResult && steps > 0)
        {
            moveResult = enumerator.MoveNext();
            steps--;
        }
        return moveResult;
    }

    public static void Forget(this Task task)
    {
    }

    public static bool In<T>(this T obj, params T[] objects)
    {
        return objects.Contains(obj);
    }

    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }

        return false;
    }

    public static bool HasCustomAttribute<T>(this Type type) where T : Attribute => type.GetCustomAttribute<T>() != null;

    public static bool IsGenericList(this PropertyInfo prop)
    {
        var type = prop.PropertyType;
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    public static Type GetGenericListType(this MemberExpression memberExpression)
    {
        return memberExpression.Type.GetGenericArguments()[0];
    }

    public static string GetDescription(this Enum element)
    {
        var type = element.GetType();
        var memberInfo = type.GetMember(element.ToString());

        if (memberInfo.Length > 0)
        {
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return ((DescriptionAttribute)attributes[0]).Description;
            }
        }
        return element.ToString();
    }

    public static object FromDescription(this Type type, string value)
    {
        foreach (Enum @enum in Enum.GetValues(type))
        {
            if (@enum.GetDescription().Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return @enum;
            }
        }

        throw new ArgumentException($"{value} isn't a member of {type.Name}");
    }

    public static long IndexOf(this Stream stream, string value, long startIndex, IndexMode mode)
    {
        var index = stream.IndexOf(value, startIndex);
        if (index >= 0)
        {
            return mode switch
            {
                IndexMode.LineStart => stream.LastIndexOf(Environment.NewLine, index - 1) + Environment.NewLine.Length,
                IndexMode.LineEnd => stream.IndexOf(Environment.NewLine, index + 1),
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };
        }
        return -1;
    }
    
    public static long IndexOfAny(this Stream stream, IEnumerable<string> values, long startIndex, IndexMode mode, out (string? Value, long Position) result)
    {
        result = (null, -1);
        long next = -1;

        foreach (var value in values)
        {
            var index = stream.IndexOf(value, startIndex);
            if (index >= 0 && (result.Value is null || index < result.Position))             
            {
                result = (
                    value,
                    mode switch
                    {
                        IndexMode.LineStart => stream.LastIndexOf(Environment.NewLine, index - 1) + Environment.NewLine.Length,
                        IndexMode.LineEnd => stream.IndexOf(Environment.NewLine, index + 1),
                        _ => throw new ArgumentOutOfRangeException(nameof(mode)),
                    });
                next = result.Position;
            }            
        }

        return next;
    }

    internal static string? GetCurrentLine(this Stream stream, ref long position, IndexMode positionType)
    {
        long start = positionType == IndexMode.LineStart ? position : (stream.LastIndexOf(Environment.NewLine, position - 1) + Environment.NewLine.Length);
        long end = positionType == IndexMode.LineStart ? stream.IndexOf(Environment.NewLine, start + 1) : position;

        if (start >= 0 && end >= 0)
        {
            stream.Seek(start, SeekOrigin.Begin);
            Span<byte> memory = new(new byte[(int)(end - start)]);
            stream.ReadExactly(memory);
            stream.Seek(position, SeekOrigin.Begin);

            position = positionType switch
            {
                IndexMode.LineStart => end + Environment.NewLine.Length,
                IndexMode.LineEnd => start,
                _ => position
            };
            return Encoding.UTF8.GetString(memory);
        }

        return null;
    }

    public static IEnumerable<string> GetLines(this string str)
    {
        using var sr = new StringReader(str);
        string? line;
        while ((line = sr.ReadLine()) != null)
            yield return line;
    }

    public static IFight? GetFight(this IFightStart start)
    {
        return start switch
        {
            EncounterStart raidStart when start.GetType() == typeof(EncounterStart) => new BossEncounter(raidStart),
            ArenaMatchStart arenaStart when start.GetType() == typeof(ArenaMatchStart) => new ArenaMatch(arenaStart),
            ChallengeModeStart challengeStart when start.GetType() == typeof(ChallengeModeStart) => new ChallengeModeEncounter(challengeStart),
            _ => null
        };
    }

    public static bool Implements<T>(this Type type)
    {        
        return typeof(T).IsAssignableFrom(type);
    }

    private static readonly string[] combatLogTimestampFormats =
    [
        "M/d HH:mm:ss.fff",
        "M/d HH:mm:ss.fffff",
    ];

    public static DateTime GetTimestamp(this string dateTimeString) =>
        DateTime.TryParseExact(dateTimeString, combatLogTimestampFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var timestamp)
            ? timestamp
            : DateTime.MinValue;

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
