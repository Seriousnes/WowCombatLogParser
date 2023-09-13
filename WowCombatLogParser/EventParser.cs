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
    public static async Task ParseCombatLogEvent<T>(T instance, IList<ICombatLogDataField> data, IEventGenerator generator) where T : class, ICombatLogEventComponent
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

    private static void GetCombatLogEventProperties(ClassMap classMap, List<InstancePropertyInfo> values, ICombatLogEventComponent combatLogEventComponent, IEventGenerator generator)
    {
        foreach (var prop in classMap.Properties)
        {
            if (prop.HasCustomAttribute<IsSingleDataFieldAttribute>() || prop.IsGenericList())
            {
                values.Add(new InstancePropertyInfo(combatLogEventComponent, prop));
            }
            else if (typeof(ICombatLogEventComponent).IsAssignableFrom(prop.PropertyType))
            {
                if (prop.GetValue(combatLogEventComponent) is CombatLogEventComponent nestedEvent)
                {
                    var nestedClassMap = generator.GetClassMap(nestedEvent.GetType());
                    GetCombatLogEventProperties(nestedClassMap, values, nestedEvent, generator);
                }
            }
            else
            {
                values.Add(new InstancePropertyInfo(combatLogEventComponent, prop));
            }
        } 
    }

    private static void SetPropertyValue(InstancePropertyInfo _this, ICombatLogDataField data, IEventGenerator generator)
    {
        if (_this.Property.IsGenericList())
        {
            SetListValue(_this, data, generator).Wait();
        }
        else
        {
            if (data is CombatLogDataFieldCollection CombatLogDataFieldCollection)
            {
                var instance = _this.GetPropertyInstance<ICombatLogEventComponent>();
                if (instance != null)
                {
                    ParseCombatLogEvent(instance, CombatLogDataFieldCollection.Children, generator).Wait();
                }
            }
            else
            {
                _this.Property.SetValue(_this.Instance, Conversion.GetValue(data, _this.Property.PropertyType));
            }
        }        
    }

    internal static async Task ParseMinimal<T>(T unparsedEvent, IList<ICombatLogDataField> data, IEventGenerator generator) where T : class, ICombatLogEventComponent
    {
        var propertiesToParse = generator.GetClassMap(unparsedEvent.GetType()).Properties
            .Take(2)
            .Select(p => new InstancePropertyInfo(unparsedEvent, p))
            .Zip(data, (p, f) => new Task(() => SetPropertyValue(p, f, generator)))
            .ToList();

        await Parallel.ForEachAsync(propertiesToParse, async (task, ct) => await task.WaitAsync(ct));
    }

    private static async Task SetListValue(InstancePropertyInfo _this, ICombatLogDataField data, IEventGenerator generator)
    {
        // instance of the list
        var instance = _this.Property.GetValue(_this.Instance);
        // any list will have the data as a CombatLogDataFieldCollection with each child representing a single list entry
        var listData = ((CombatLogDataFieldCollection)data).Children;
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
            var item = (ICombatLogEventComponent)classMap.Constructor();
            if (field is CombatLogDataFieldCollection listItemData)
            {
                await ParseCombatLogEvent(item, listItemData.Children, generator);
            }
            else
            {
                await ParseCombatLogEvent(item, new[] { field }, generator);
            }
                
            addMethod?.Invoke(instance, new[] { item });
        }
    }

    private static IList<ICombatLogDataField> CollateKeyValuePairs(IList<ICombatLogDataField> data)
    {
        var result = data
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / 2)
            .Select(x =>
            {
                var group = new CombatLogDataFieldCollection { OpeningBracket = '[', };                
                x.Select(x => x.Value).ToList().ForEach(x => group.AddChild(x));
                return (ICombatLogDataField)group;
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
