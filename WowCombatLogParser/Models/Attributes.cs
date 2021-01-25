using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldOrderAttribute : Attribute
    {
        public FieldOrderAttribute(int value)
        {
            Id = value;
        }

        public int Id { get; private set; }
    }

    public class AffixAttribute : Attribute
    {
        public AffixAttribute(string value)
        {
            Name = value;
        }

        public string Name { get; private set; }
    }

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
}
