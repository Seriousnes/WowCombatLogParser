using System;

namespace WoWCombatLogParser;

/// <summary>
/// Base class for all <see cref="CombatLogEvent"/> types.
/// </summary>
public abstract class CombatLogEvent : CombatLogEventComponent, ICombatLogEvent
{
    private static int _count = 0;

    public CombatLogEvent()
    {
        Id = _count++;
    }

    /// <summary>
    /// Used for sorting to ensure multiple events with the same timestamp maintain their correct position.
    /// </summary>
    [NonData]
    public int Id { get; private set; }
    /// <summary>
    /// Date/Time the event occured
    /// </summary>
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// Name of the event. Not to be confused with the EVENT_TYPE field in the combat log file (see <see cref="DiscriminatorAttribute"/>).
    /// </summary>
    [NonData]
    public string EventName => GetType().Name;
}