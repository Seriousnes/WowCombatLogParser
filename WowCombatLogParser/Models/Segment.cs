using System;
using WoWCombatLogParser.IO;

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
public sealed class Segment(ICombatLogFileContext context, long start, int length)
{
    public ICombatLogFileContext Context { get; } = context;
    public long StartOffset { get; } = start;
    public int Length { get; } = length;

    /// <summary>
    /// Event representing the beginning of this segment
    /// </summary>
    public IFightStart? Start { get; internal set; }
    /// <summary>
    /// Event represending the end of this segment
    /// </summary>
    public IFightEnd? End { get; internal set; }
}