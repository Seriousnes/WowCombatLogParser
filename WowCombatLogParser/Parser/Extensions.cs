using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser.Utility
{
    public static class Extensions
    {
        public static (bool Success, IEnumerator<IField> Enumerator, bool EndOfParent, bool Dispose) GetEnumeratorForProperty(this IEnumerator<IField> data)
        {
            if (data.Current is GroupField groupData)
            {
                var enumerator = groupData.Children.GetEnumerator();
                return (enumerator.MoveNext(), enumerator, !data.MoveNext(), true);
            }

            return (true, data, false, false);
        }
    }
}
