using System;
using System.ComponentModel;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Utility
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum element)
        {
            var type = element.GetType();
            var memberInfo = type.GetMember(element.ToString());

            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return element.ToString();
        }

        public static object FromDescription(string value, Type type)
        {
            foreach (Enum @enum in Enum.GetValues(type))
            {
                if (@enum.GetDescription().Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return @enum;
                }
            }

            throw new ArgumentException($"{value} isn't a member of {type.Name}");
        }
    }
}
