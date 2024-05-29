using System;

namespace WoWCombatLogParser;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class OptionalAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
internal class IsSingleDataFieldAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
internal class KeyValuePairAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
internal class NonDataAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
internal class CombatLogVersionAttribute(CombatLogVersion combatLogVersion) : Attribute
{
    public CombatLogVersion Value { get; } = combatLogVersion;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal class DiscriminatorAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}