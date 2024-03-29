﻿using System;
using System.Collections.Generic;

namespace WoWCombatLogParser.Common.Models;

[AttributeUsage(AttributeTargets.Class)]
public class AffixAttribute : Attribute
{
    public AffixAttribute(string value)
    {
        Name = value;
    }

    public string Name { get; private set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class PrefixAttribute : AffixAttribute
{
    public PrefixAttribute(string value) : base(value)
    {
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class SuffixAttribute : AffixAttribute
{
    public SuffixAttribute(string value) : base(value)
    {
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SuffixAllowedAttribute : Attribute
{
    public SuffixAllowedAttribute(params Type[] suffix)
    {
        Suffixes.AddRange(suffix);
    }

    public List<Type> Suffixes { get; } = new List<Type>();
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SuffixNotAllowedAttribute : Attribute
{
    public SuffixNotAllowedAttribute(params Type[] suffix)
    {
        Suffixes.AddRange(suffix);
    }

    public List<Type> Suffixes { get; } = new List<Type>();
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class IsSingleDataFieldAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class KeyValuePairAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class OffsetAttribute : Attribute
{
    public OffsetAttribute(int value)
    {
        Value = value;
    }

    public int Value { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class NonDataAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class KeyAttribute : Attribute
{
    /// <summary>
    /// Identifies a property as being a key
    /// </summary>
    /// <param name="fields">Number of fields present in the object, inclusive of the key</param>
    public KeyAttribute(int fields)
    {
        Fields = fields;
    }

    public int Fields { get; set; }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class CombatLogVersionAttribute : Attribute
{
    public CombatLogVersionAttribute(CombatLogVersion combatLogVersion)
    {
        Value = combatLogVersion;
    }

    public CombatLogVersion Value { get; }
}
