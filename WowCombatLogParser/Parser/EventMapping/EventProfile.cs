using System;

namespace WoWCombatLogParser.Parser.EventMapping;

/// <summary>
/// Some events (such as <see cref="SpellAbsorbed.ExtraSpell"/>) have optional values that are not at the end of the event string.
/// Inherit from this class and implement 
/// </summary>
/// <remarks>
/// Can be implemented for any type that inherits from <see cref="CombatLogEventComponent"/>.
/// </remarks>
internal abstract class EventProfile
{
    /// <summary>
    /// The type of the combat log event this mapping profile refers to
    /// </summary>
    public abstract Type EventType { get; }

    /// <summary>
    /// Custom mapping for the specifed component.
    /// </summary>
    /// <param name="mapper">Instance of an <see cref="ICombatLogEventMapper"/></param>    
    /// <returns>Returns the value of the last data field accessed.</returns>
    public virtual CombatLogEventMapping GetMapping(CombatLogEventMapper mapper)
    {
        return (CombatLogEventComponent combatLogEvent, List<ICombatLogDataField> data, int index) =>
        {
            return index;
        };
    }
}
