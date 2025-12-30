using System;
using System.Threading;
using System.Threading.Tasks;

namespace WoWCombatLogParser;

/// <summary>
/// <para>
/// A <see cref="Segment"/> represents a continuous region of the combat log file, starting at byte position <paramref name="start"/> 
/// with a length of <paramref name="length"/>.
/// </para>
/// <para>
/// <see cref="Content"/> is lazy loaded to support loading multiple segments in parallel.
/// </para>
/// </summary>
/// <remarks>
/// Assumes the file's content from <paramref name="start"/> to <paramref name="start"/>+<paramref name="length"/> is unmodified from when 
/// the instance of <see cref="Segment"/> was created.
/// </remarks>
public sealed class Segment(long start, int length)
{
    public long StartOffset { get; } = start;
    public int Length { get; } = length;

    public IReadOnlyList<CombatLogEvent>? Events { get; internal set; }

    /// <summary>
    /// Event representing the beginning of this segment
    /// </summary>
    public IFightStart? Start { get; internal set; }
    /// <summary>
    /// Event represending the end of this segment
    /// </summary>
    public IFightEnd? End { get; internal set; }

    public IFight? ToFight()
    {
        if (Start is null)
        {
            return null;
        }

        if (Start.GetFight() is not { } fight)
        {
            return null;
        }

        if (Events is null)
        {
            throw new InvalidOperationException("Segment must be parsed before calling ToFight.");
        }

        fight.Range = (StartOffset, StartOffset + Length);

        for (var i = 0; i < Events.Count; i++)
        {
            fight.AddEvent(Events[i]);
        }

        if (End is CombatLogEvent endEvent)
        {
            fight.AddEvent(endEvent);
        }

        fight.Sort();
        return fight;
    }

    public async Task<IFight?> ToFightAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ToFight();
    }
}