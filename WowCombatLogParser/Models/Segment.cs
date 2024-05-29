using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using WoWCombatLogParser.Utility;

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
public class Segment(string filename, long start, long length)
{
    private readonly object _lock = new();
    private List<string>? lines;

    /// <summary>
    /// Each item represents an unparsed <see cref="CombatLogEvent"/> string
    /// </summary>
    public List<string> Content
    {
        get
        {
            Load();
            return lines ?? [];
        }
    }

    /// <summary>
    /// Event representing the beginning of this segment
    /// </summary>
    public IFightStart? Start { get; internal set; }
    /// <summary>
    /// Event represending the end of this segment
    /// </summary>
    public IFightEnd? End { get; internal set; }

    internal void Load()
    {
        lock(_lock)
        {
            if (lines != null) return;
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, StreamExtensions.GetBufferSize(filename), FileOptions.RandomAccess);
            fs.Seek(start, SeekOrigin.Begin);
            Span<byte> memory = new(new byte[(int)length]);
            fs.Read(memory);
            lines = Encoding.UTF8.GetString(memory.ToArray()).GetLines().ToList();
        }
    }
}