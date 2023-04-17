using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WoWCombatLogParser
{
    public interface IField
    {
        IField Parent { get; set; }
        Range Range { get; set; }
    }

    [DebuggerDisplay("{ToString()}")]
    public class TextField : IField
    {
        private StringBuilder _text = new();
        public virtual string Content => _text.ToString();
        public IField Parent { get; set; }
        public Range Range { get; set; } = Range.EmptyRange;

        public virtual void Append(string value) => _text.Append(value);

        public virtual void Append(char value)
        {
            if (value == '{') Append("{");
            if (value == '}') Append("}");
            Append(value.ToString());
        }

        public override string ToString() => _text.ToString();

        public static implicit operator string(TextField field)
        {
            return field.ToString();
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class QuotedTextField : TextField
    {
        public override string ToString()
        {
            return $"\"{base.ToString()}\"";
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public class GroupField : IField
    {
        private static readonly Dictionary<char, char> bracketPairs = new Dictionary<char, char>()
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

        public override string ToString()
        {
            return Children.Count > 0 ? $"{OpeningBracket}{string.Join(",", Children.Select(x => x.ToString()).ToArray())}{ClosingBracket}" : "";
        }
    }
}
