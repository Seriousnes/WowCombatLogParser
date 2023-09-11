using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WoWCombatLogParser;

public interface ICombatLogDataField
{
    ICombatLogDataField Parent { get; set; }
    Range Range { get; set; }
}


[DebuggerDisplay("{ToString()}")]
public class CombatLogTextField : ICombatLogDataField
{
    private StringBuilder _text = new();
    public virtual string Content => _text.ToString();
    public ICombatLogDataField Parent { get; set; }
    public Range Range { get; set; } = Range.EmptyRange;

    public virtual void Append(string value) => _text.Append(value);

    public virtual void Append(char value)
    {
        if (value == '{') Append("{");
        if (value == '}') Append("}");
        Append(value.ToString());
    }

    public override string ToString() => _text.ToString();

    public static implicit operator string(CombatLogTextField field)
    {
        return field.ToString();
    }
}

[DebuggerDisplay("{ToString()}")]
public class QuotedCombatLogTextField : CombatLogTextField
{
    public override string ToString()
    {
        return $"\"{base.ToString()}\"";
    }
}

[DebuggerDisplay("{ToString()}")]
public class CombatLogDataFieldCollection : ICombatLogDataField
{
    private static readonly Dictionary<char, char> bracketPairs = new Dictionary<char, char>()
    {
        { '(', ')' },
        { '[', ']' },
        { '{', '}' }
    };

    private char openingBracket;
    public IList<ICombatLogDataField> Children { get; } = new List<ICombatLogDataField>();

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
    public ICombatLogDataField Parent { get; set; }
    public Range Range { get; set; } = new Range(0, 0);

    public virtual void AddChild(ICombatLogDataField child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public override string ToString()
    {
        return Children.Count > 0 ? $"{OpeningBracket}{string.Join(",", Children.Select(x => x.ToString()).ToArray())}{ClosingBracket}" : "";
    }
}
