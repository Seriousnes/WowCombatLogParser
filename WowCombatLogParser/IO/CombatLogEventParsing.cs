using System;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.IO;

internal static class CombatLogEventParsing
{
    internal static IReadOnlyList<CombatLogEvent> ParseSequential(ReadOnlySpan<byte> data, Func<string, CombatLogEvent?> parseLine)
    {
        var events = new List<CombatLogEvent>(capacity: 1024);
        int i = 0;

        while (i < data.Length)
        {
            int lineBreak = data[i..].IndexOf((byte)'\n');
            int lineEndExclusive = lineBreak >= 0 ? i + lineBreak : data.Length;

            var line = data[i..lineEndExclusive];
            if (line.Length > 0 && line[^1] == (byte)'\r')
            {
                line = line[..^1];
            }

            if (!line.IsEmpty)
            {
                var lineText = Encoding.UTF8.GetString(line);
                if (parseLine(lineText) is { } combatLogEvent)
                {
                    events.Add(combatLogEvent);
                }
            }

            if (lineBreak < 0)
            {
                break;
            }

            i = lineEndExclusive + 1;
        }

        return events;
    }

    internal static IReadOnlyList<CombatLogEvent> ParseParallelFromMemory(ReadOnlyMemory<byte> data, Func<string, CombatLogEvent?> parseLine)
    {
        var ranges = new List<(int Start, int Length)>(capacity: 1024);

        var span = data.Span;
        int p = 0;
        while (p < span.Length)
        {
            int lineBreak = span[p..].IndexOf((byte)'\n');
            int lineEndExclusive = lineBreak >= 0 ? p + lineBreak : span.Length;

            int lineLength = lineEndExclusive - p;
            if (lineLength > 0 && span[p + lineLength - 1] == (byte)'\r')
            {
                lineLength -= 1;
            }

            if (lineLength > 0)
            {
                ranges.Add((p, lineLength));
            }

            if (lineBreak < 0)
            {
                break;
            }

            p = lineEndExclusive + 1;
        }

        var results = new CombatLogEvent?[ranges.Count];
        Parallel.For(0, ranges.Count, idx =>
        {
            var (start, length) = ranges[idx];
            var lineText = Encoding.UTF8.GetString(data.Span.Slice(start, length));
            results[idx] = parseLine(lineText);
        });

        var output = new List<CombatLogEvent>(results.Length);
        for (int idx = 0; idx < results.Length; idx++)
        {
            if (results[idx] is { } e)
            {
                output.Add(e);
            }
        }

        return output;
    }

    internal static unsafe IReadOnlyList<CombatLogEvent> ParseParallelFromPointer(byte* basePtr, int length, Func<string, CombatLogEvent?> parseLine)
    {
        var ranges = new List<(int Start, int Length)>(capacity: 1024);

        int p = 0;
        while (p < length)
        {
            var remaining = new ReadOnlySpan<byte>(basePtr + p, length - p);
            int lineBreak = remaining.IndexOf((byte)'\n');
            int lineEndExclusive = lineBreak >= 0 ? p + lineBreak : length;

            int lineLength = lineEndExclusive - p;
            if (lineLength > 0 && *(basePtr + p + lineLength - 1) == (byte)'\r')
            {
                lineLength -= 1;
            }

            if (lineLength > 0)
            {
                ranges.Add((p, lineLength));
            }

            if (lineBreak < 0)
            {
                break;
            }

            p = lineEndExclusive + 1;
        }

        var results = new CombatLogEvent?[ranges.Count];
        Parallel.For(0, ranges.Count, idx =>
        {
            var (start, lineLength) = ranges[idx];
            var line = new ReadOnlySpan<byte>(basePtr + start, lineLength);
            var lineText = Encoding.UTF8.GetString(line);
            results[idx] = parseLine(lineText);
        });

        var output = new List<CombatLogEvent>(results.Length);
        for (int idx = 0; idx < results.Length; idx++)
        {
            if (results[idx] is { } e)
            {
                output.Add(e);
            }
        }

        return output;
    }
}
