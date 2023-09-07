using System;

namespace WoWCombatLogParser.Models;

public class CombatLogParseException : Exception
{
    public CombatLogParseException(string property, Type type, string value)
    {
        Property = property;
        TypeExpected = type;
        Value = value;
    }

    public string Property { get; set; }
    public Type TypeExpected { get; set; }
    public string Value { get; set; }

    public override string ToString()
    {
        return $"Unable to convert {Value} to the required type ({TypeExpected.Name}).";
    }
}
