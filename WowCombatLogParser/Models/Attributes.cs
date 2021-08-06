using System;

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
