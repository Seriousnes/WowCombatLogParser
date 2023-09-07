using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser;

public static class EventParser
{
    public static async Task ParseCombatLogEventAsync<T>(T instance, IList<IField> data, IEventGenerator generator) where T : class, IEvent
    {
        var classMap = generator.GetClassMap(instance.GetType());
        if (classMap.CustomAttributes.OfType<KeyValuePairAttribute>().Any())
        {
            data = CollateKeyValuePairs(data);
        }
        var flatMap = new List<InstancePropertyInfo>();
        GetCombatLogEventProperties(classMap, flatMap, instance, generator);
        var actions = flatMap.Zip(data, (p, f) => new Action(() => SetPropertyValue(p, f, generator))).ToList();

        actions.ForEach(action => action());
    }    

    private static void GetCombatLogEventProperties(ClassMap classMap, List<InstancePropertyInfo> values, IEvent @event, IEventGenerator generator)
    {
        foreach (var prop in classMap.Properties)
        {
            if (prop.HasCustomAttribute<IsSingleDataFieldAttribute>() || prop.IsGenericList())
            {
                values.Add(new InstancePropertyInfo(@event, prop));
            }
            else if (typeof(IEvent).IsAssignableFrom(prop.PropertyType))
            {
                if (prop.GetValue(@event) is Event nestedEvent)
                {
                    var nestedClassMap = generator.GetClassMap(nestedEvent.GetType());
                    GetCombatLogEventProperties(nestedClassMap, values, nestedEvent, generator);
                }
            }
            else
            {
                values.Add(new InstancePropertyInfo(@event, prop));
            }
        } 
    }

    private static void SetPropertyValue(InstancePropertyInfo _this, IField data, IEventGenerator generator)
    {
        if (_this.Property.IsGenericList())
        {
            SetListValue(_this, data, generator).Wait();
        }
        else
        {
            if (data is GroupField groupField)
            {
                var instance = _this.GetPropertyInstance<IEvent>();
                if (instance != null)
                {
                    ParseCombatLogEventAsync(instance, groupField.Children, generator).Wait();
                }
            }
            else
            {
                _this.Property.SetValue(_this.Instance, Conversion.GetValue(data, _this.Property.PropertyType));
            }
        }        
    }

    private static async Task SetListValue(InstancePropertyInfo _this, IField data, IEventGenerator generator)
    {
        // instance of the list
        var instance = _this.Property.GetValue(_this.Instance);
        // any list will have the data as a GroupField with each child representing a single list entry
        var listData = ((GroupField)data).Children;
        // the type of each item in the list
        var listItemType = _this.Property.GetGenericListType();
        // the type defintion for the list
        var listType = typeof(List<>).MakeGenericType(listItemType);
        // add method
        var addMethod = listType.GetMethod("Add");
        // class map of each list item type
        var classMap = generator.GetClassMap(listItemType);

        if (_this.Property.HasCustomAttribute<KeyValuePairAttribute>())
            listData = CollateKeyValuePairs(listData);

        foreach (var field in listData) 
        {
            var item = (IEvent)classMap.Constructor();
            if (field is GroupField listItemData)
            {
                await ParseCombatLogEventAsync(item, listItemData.Children, generator);
            }
            else
            {
                await ParseCombatLogEventAsync(item, new[] { field }, generator);
            }
                
            addMethod?.Invoke(instance, new[] { item });
        }
    }

    private static IList<IField> CollateKeyValuePairs(IList<IField> data)
    {
        var result = data
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / 2)
            .Select(x =>
            {
                var group = new GroupField { OpeningBracket = '[', };                
                x.Select(x => x.Value).ToList().ForEach(x => group.AddChild(x));
                return (IField)group;
            })
            .ToList();
        return result;
    }
}

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
