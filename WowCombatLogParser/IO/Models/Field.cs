using System;
using System.Linq;
using System.Text;

namespace WoWCombatLogParser;

public interface ICombatLogDataField
{
    ICombatLogDataField? Parent { get; set; }
}


[DebuggerDisplay("{ToString()}")]
internal class CombatLogTextField : ICombatLogDataField
{
    private ReadOnlyMemory<char>? data;
    private StringBuilder? _text = new();

    public virtual string Content => ToString();

    public ICombatLogDataField? Parent { get; set; }

    public virtual void Append(string value) => _text?.Append(value);

    public virtual void Append(char value)
    {
        if (value.In('{', '}'))
            _text?.Append(value);
        _text?.Append(value);
    }

    public virtual bool IsFinalised => _text is null;

    public virtual void Finalise()
    {
        data = _text?.ToString().AsMemory();
        _text = null;
    }

    public override string ToString() => data?.ToString() ?? throw new InvalidOperationException($"Attempt to call ToString() before Finalise()", new NullReferenceException());

    public static implicit operator string(CombatLogTextField field)
    {
        return field.ToString();
    }
}

[DebuggerDisplay("{ToString()}")]
internal class QuotedCombatLogTextField : CombatLogTextField
{
    public override string ToString()
    {
        return $"\"{base.ToString()}\"";
    }
}

[DebuggerDisplay("{ToString()}")]
internal class CombatLogDataFieldCollection : ICombatLogDataField
{
    private static readonly Dictionary<char, char> bracketPairs = new()
    {
        { '(', ')' },
        { '[', ']' },
        { '{', '}' }
    };

    private char openingBracket;
    public List<ICombatLogDataField> Children { get; } = [];

    public virtual char OpeningBracket
    {
        get => openingBracket;
        set
        {
            openingBracket = value;
            ClosingBracket = bracketPairs.TryGetValue(value, out char closingBracket) ? closingBracket : throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
    public virtual char ClosingBracket { get; private set; }
    public ICombatLogDataField? Parent { get; set; }

    public virtual void AddChild(ICombatLogDataField child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public override string ToString()
    {
        return Children.Count > 0 ? $"{OpeningBracket}{string.Join(",", [.. Children.Select(x => x.ToString())])}{ClosingBracket}" : "";
    }
}
