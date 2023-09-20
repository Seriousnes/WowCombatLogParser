using System.Reflection;

namespace WoWCombatLogParser;

[DebuggerDisplay("{Instance.GetType().Name} - {Property.Name}")]
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
