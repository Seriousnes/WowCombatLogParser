using System;
using System.Collections.Generic;

namespace WoWCombatLogParser.SourceGenerator.Models;

[AttributeUsage(AttributeTargets.Class)]
internal class AffixAttribute(string value) : Attribute
{
    public string Name { get; private set; } = value;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal class PrefixAttribute(string value) : AffixAttribute(value)
{
}

[AttributeUsage(AttributeTargets.Class)]
internal class SuffixAttribute(string value) : AffixAttribute(value)
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