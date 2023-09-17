using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Parser;

internal class InstancePropertyInfo
{
    public InstancePropertyInfo(object instance, PropertyInfo propertyInfo)
    {
        Instance = instance;
        Property = propertyInfo;
    }

    public object Instance { get; set; }
    public PropertyInfo Property { get; set; }
    public T? GetPropertyInstance<T>() => (T?)Property.GetValue(Instance);
}
