using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WoWCombatLogParser.Models
{
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

        public List<Type> Suffixes { get; set; } = new();
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SuffixNotAllowedAttribute : Attribute
    {
        public SuffixNotAllowedAttribute(params Type[] suffix)
        {
            Suffixes.AddRange(suffix);
        }

        public List<Type> Suffixes { get; set; } = new();
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
}
