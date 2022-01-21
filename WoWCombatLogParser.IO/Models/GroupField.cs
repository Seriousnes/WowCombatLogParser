using System.Diagnostics;

namespace WoWCombatLogParser.IO
{
    public interface IField
    {
        IField Parent { get; set; }
        Range Range { get; set; }
        string AsString();
    }

    [DebuggerDisplay("{AsString()}")]
    public class TextField : IField
    {
        public virtual string Content { get; set; }
        public IField Parent { get; set; }
        public Range Range { get; set; } = Range.EmptyRange;
        public virtual void Append(string value)
        {
            Content += value;
        }

        public virtual void Append(char value)
        {
            if (value == '{') Append("{");
            if (value == '}') Append("}");
            Append(value.ToString());
        }

        public virtual string AsString()
        {
            return Content;
        }
    }

    [DebuggerDisplay("{AsString()}")]
    public class QuotedTextField : TextField
    {
        public override string AsString()
        {
            return $"\"{base.AsString()}\"";
        }
    }

    [DebuggerDisplay("{AsString()}")]
    public class GroupField : IField
    {
        private static readonly Dictionary<char, char> bracketPairs = new()
        {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' }
        };

        private char openingBracket;
        public IList<IField> Children { get; } = new List<IField>();

        public virtual char OpeningBracket
        {
            get => openingBracket;
            set
            {
                openingBracket = value;
                ClosingBracket = bracketPairs.TryGetValue(value, out char closingBracket) ? closingBracket : throw new ArgumentOutOfRangeException();
            }
        }
        public virtual char ClosingBracket { get; private set; }
        public IField Parent { get; set; }
        public Range Range { get; set; } = new Range(0, 0);

        public virtual void AddChild(IField child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public virtual string AsString()
        {
            return Children.Count > 0 ? $"{OpeningBracket}{string.Join(',', Children.Select(x => x.AsString()).ToArray())}{ClosingBracket}" : "";
        }
    }
}
