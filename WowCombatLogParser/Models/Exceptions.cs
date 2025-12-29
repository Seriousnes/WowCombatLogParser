using System;

namespace WoWCombatLogParser;

public class CombatLogParseException(string property, Type type, string value) : Exception
{
    public string Property { get; set; } = property;
    public Type TypeExpected { get; set; } = type;
    public string Value { get; set; } = value;

    public override string ToString()
    {
        return $"Unable to convert {Value} to the required type ({TypeExpected.Name}).";
    }
}
