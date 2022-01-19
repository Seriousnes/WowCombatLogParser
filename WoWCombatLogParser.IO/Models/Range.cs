using System.Diagnostics;

namespace WoWCombatLogParser.IO
{
    [DebuggerDisplay("{Start} - {End}")]
    public class Range
    {
        public Range(int start, int end)
        {
            Start = start;
            End = end;            
        }

        public int Start { get; set; }
        public int End { get; set; }

        public static readonly Range EmptyRange = new(0, 0);
        public bool IsEmpty() => IsEmpty(this);
        public static bool IsEmpty(Range range) => range.Start == range.End;
        public bool ContainsPosition(int position) => ContainsPosition(this, position); 
        public static bool ContainsPosition(Range range, int position) => position >= range.Start && position <= range.End;
        public static bool StrictContainsPosition(Range range, int position) => position > range.Start && position < range.End;
        public bool ContainsRange(Range range) => ContainsRange(this, range);
        public static bool ContainsRange(Range range, Range otherRange) => range.Start <= otherRange.Start && range.End >= otherRange.End;
        public bool StrictContainsRange(Range range) => StrictContainsRange(this, range);
        public static bool StrictContainsRange(Range range, Range otherRange) => range.Start < otherRange.Start && range.End > otherRange.End;
        public Range Plus(Range range) => Plus(this, range);
        public static Range Plus(Range a, Range b) => new Range(Math.Min(a.Start, b.Start), Math.Max(a.End, b.End));
        public Range? Intersect(Range range) => Intersect(this, range);
        
        public static Range? Intersect(Range a, Range b)
        {
            int start = Math.Max(a.Start, b.Start);
            int end = Math.Min(a.Start, b.End);

            if (start > end) return null;
            return new Range(start, end);
        }

        public static bool AreIntersectingOrTouching(Range a, Range b)
        {
            if (a.End < b.Start) return false;
            if (b.End < a.Start) return false;
            return true;
        }

        public static bool AreIntersecting(Range a, Range b)
        {
            if (a.End <= b.Start) return false;
            if (b.End <= a.Start) return false;
            return true;
        }

        public static int CompareRangesUsingStarts(Range? a, Range? b)
        {
            if (a is not null && b is not null)
            {
                if (a.Start == b.Start)
                {
                    return a.End - b.End;
                }
                return a.Start - b.Start;
            }

            return (a is not null ? 1 : 0) - (b is not null ? 1 : 0);
        }
    }
}
