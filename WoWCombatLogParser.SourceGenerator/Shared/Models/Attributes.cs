using System;
using System.Collections.Generic;

namespace WoWCombatLogParser.Models;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OptionalAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class IsSingleDataFieldAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class KeyValuePairAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class NonDataAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class CombatLogVersionAttribute(CombatLogVersion combatLogVersion) : Attribute
{
    public CombatLogVersion Value { get; } = combatLogVersion;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class DiscriminatorAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}