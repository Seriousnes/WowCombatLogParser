using System;
using System.Collections.Generic;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.SourceGenerator.Models;

[AttributeUsage(AttributeTargets.Class)]
public class AffixAttribute(string value) : Attribute
{
    public string Name { get; private set; } = value;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class PrefixAttribute(string value) : AffixAttribute(value)
{
}

[AttributeUsage(AttributeTargets.Class)]
public class SuffixAttribute(string value) : AffixAttribute(value)
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class SuffixAllowedAttribute : Attribute
{
    public SuffixAllowedAttribute(params Type[] suffix) => Suffixes.AddRange(suffix);

    public List<Type> Suffixes { get; } = [];
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class SuffixNotAllowedAttribute : Attribute
{
    public SuffixNotAllowedAttribute(params Type[] suffix) => Suffixes.AddRange(suffix);

    public List<Type> Suffixes { get; } = [];
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class IsSingleDataFieldAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
internal class KeyValuePairAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class NonDataAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
internal class CombatLogVersionAttribute(CombatLogVersion combatLogVersion) : Attribute
{
    public CombatLogVersion Value { get; } = combatLogVersion;
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OptionalAttribute : Attribute { }
