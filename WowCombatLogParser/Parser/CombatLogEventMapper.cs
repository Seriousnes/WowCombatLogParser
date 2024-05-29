using ExpressionDebugger;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WoWCombatLogParser.Parser;
using WoWCombatLogParser.Parser.EventMapping;
using WoWCombatLogParser.SourceGenerator.Events;
using WoWCombatLogParser.Utility;
using WoWCombatLogParser.SourceGenerator.Models;
using WoWCombatLogParser.IO;
using static WoWCombatLogParser.IO.CombatLogFieldReader;
using static System.Linq.Expressions.Expression;

namespace WoWCombatLogParser;

public interface ICombatLogEventMapper
{
    CombatLogEvent? GetCombatLogEvent(string line);
    T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent;
    void SetCombatLogVersion(string combatLogVersion);
    void SetCombatLogVersion(CombatLogVersion combatLogVersion);
}

internal class CombatLogEventMapper : ICombatLogEventMapper
{
    private readonly CombatLogVersionedDictionary<string, ObjectActivator> eventConstructors = new();
    private readonly Dictionary<Type, CombatLogEventMapping> maps;
    private readonly Dictionary<Type, ObjectActivator> componentConstructors = [];
    private readonly Dictionary<string, MethodInfo> methods = new()
    {
        { nameof(CombatLogEventMapper.MapComponentAsProperties), typeof(CombatLogEventMapper).GetMethod(nameof(CombatLogEventMapper.MapComponentAsProperties), BindingFlags.Instance | BindingFlags.NonPublic)! },
        { nameof(Conversion.GetValue), typeof(Conversion).GetMethod(nameof(Conversion.GetValue), [typeof(ICombatLogDataField), typeof(Type)])! }
    };


    public CombatLogEventMapper()
    {
        var assemblyTypes = GetType().Assembly.GetTypes();

        // add any custom mapping overrides
        maps = assemblyTypes
            .Where(x => x.IsSubclassOf(typeof(EventProfile)))
            .Select(x => (EventProfile)Activator.CreateInstance(x)!)
            .ToDictionary(key => key.EventType, value => value.GetMapping(this));

        // constructors for individual events
        assemblyTypes
            .Where(i => i.GetCustomAttribute<DiscriminatorAttribute>() != null)
            .ToList()
            .ConvertAll(i => new EventAffixItem(i))
            .ForEach(p =>
            {
                // each combatlog event is expected to have a constructor with no parameters
                var constructor = p.EventType.GetConstructor([]);
                if (constructor is null) return;

                var applicableCombatLogVersions = p.EventType
                    .GetCustomAttributes<CombatLogVersionAttribute>()
                    .Select(x => x.Value)
                    .ToList();
                if (applicableCombatLogVersions.Count == 0)
                    applicableCombatLogVersions = [CombatLogVersion.Any];

                foreach (var CombatLogVersion in applicableCombatLogVersions)
                {
                    eventConstructors.TryAdd(CombatLogVersion, p.Affix.Value, GetActivator(constructor));
                }
            });
    }

    [DisallowNull]
    internal CombatLogEventMapping? this[Type index]
    {
        get
        {
            if (!maps.TryGetValue(index, out var map))
            {
                map = BuildDelegateForType(index);
            }
            return map;
        }
        set => maps[index] = value;
    }

    internal CombatLogVersionEvent? CombatLogVersionEvent { get; private set; }

    public void SetCombatLogVersion(string combatLogVersion)
    {
        CombatLogVersionEvent = new CombatLogVersionEvent(combatLogVersion);
    }

    public void SetCombatLogVersion(CombatLogVersion combatLogVersion)
    {
        CombatLogVersionEvent = new CombatLogVersionEvent(combatLogVersion);
    }

    public CombatLogEvent? GetCombatLogEvent(string line) => GetCombatLogEvent<CombatLogEvent>(line);
    public T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent => GetCombatLogEvent<T>(ReadFields(line));

    private T? GetCombatLogEvent<T>(CombatLogLineData line) where T : CombatLogEvent
    {
        var result = GetInstanceOf(line.EventType);
        if (result is { })
            this[result.GetType()]!(result, line.Data, 0);
        return result as T;
    }

    private CombatLogEvent GetInstanceOf(string eventType)
    {
        var constructor = eventConstructors[CombatLogVersionEvent!.Version, eventType] ??
            throw new CombatLogParserException(eventType, new ArgumentOutOfRangeException(nameof(eventType), $"No constructor could be found for an event of type \"{eventType}\""));
        var result = (CombatLogEvent)constructor();
        if (result is { })
        {
            return result;
        }
        throw new CombatLogParserException(eventType, new InvalidOperationException($"Failed to instantiate an instance for an event for \"{eventType}\""));
    }

    private CombatLogEventMapping? BuildDelegateForType(Type type)
    {
        // this method is recursive and previous invocations may have already added the specified type
        if (maps.TryGetValue(type, out var existing))
            return existing;

        var mapper = Constant(this);
        var p0 = Parameter(typeof(CombatLogEventComponent), "c");
        var p1 = Parameter(typeof(List<ICombatLogDataField>), "d");
        var p2 = Parameter(typeof(int), "s");

        var index = Variable(typeof(int), "i");
        var component = Variable(type, "component");
        var variables = new List<ParameterExpression>([index, component]);

        var returnLabel = Label(typeof(int), "Return");

        var field = Call(p1, typeof(List<ICombatLogDataField>).GetMethod("get_Item", [typeof(int)])!, PostIncrementAssign(index));
        var optionalFieldCheck = IfThen(
            GreaterThanOrEqual(
                index,
                MakeMemberAccess(
                    p1,
                    p1.Type.GetMember(nameof(List<ICombatLogDataField>.Count))[0]
                )
            ),
            Return(returnLabel, index)
        );

        var block = new List<Expression>(
        [
            Assign(index, p2),
            Assign(component, TypeAs(p0, type))
        ]);

        foreach (var property in type.GetTypePropertyInfo())
        {
            if (property.HasCustomAttribute<OptionalAttribute>())
            {
                block.Add(optionalFieldCheck);
            }

            if (property.IsGenericList())
            {
                var member = MakeMemberAccess(component, property);
                block.Add(
                    Call(
                        mapper,
                        nameof(MapComponentList),
                        [
                            member.GetGenericListType(),
                        ],
                        member,
                        field,
                        Constant(property.HasCustomAttribute<KeyValuePairAttribute>())
                    )
                );
            }
            else if (property.PropertyType.IsSubclassOf(typeof(CombatLogEventComponent)))
            {
                if (property.HasCustomAttribute<IsSingleDataFieldAttribute>())
                {
                    block.Add(
                        Assign(
                            index,
                            Call(
                                mapper,
                                methods[nameof(MapComponentAsProperties)],
                                MakeMemberAccess(component, property),
                                MakeMemberAccess(
                                    TypeAs(
                                        field,
                                        typeof(CombatLogDataFieldCollection)
                                    ),
                                    typeof(CombatLogDataFieldCollection).GetMember(nameof(CombatLogDataFieldCollection.Children))[0]
                                ),
                                index // step back the index one as it was previously access
                            )
                        )
                    );
                }
                else
                {
                    block.Add(
                        Assign(
                            index,
                            Call(
                                mapper,
                                methods[nameof(MapComponentAsProperties)],
                                MakeMemberAccess(component, property),
                                p1,
                                index // step back the index one as it was previously access
                            )
                        )
                    );
                }
            }
            else
            {
                block.Add(
                    Assign(
                        MakeMemberAccess(component, property),
                        Convert(
                            Call(
                                methods[nameof(Conversion.GetValue)],
                                field,
                                Constant(property.PropertyType)
                            ),
                            property.PropertyType
                        )
                    )
                );
            }
        }

        var preCompile = Lambda<CombatLogEventMapping>(
            Block(
                typeof(int),
                variables,
                [.. block, index, Label(returnLabel, index)]
            ),
            p0, p1, p2);

        CombatLogEventMapping map;

#if DEBUG
        if (Debugger.IsAttached)
        {
            var lambdaScript = preCompile.ToScript();
            map = preCompile.CompileWithDebugInfo();
        }
        else
        {
            map = preCompile.Compile();
        }
#else
        map = preCompile.Compile();
#endif

        maps.TryAdd(type, map);
        return map;
    }

    internal void MapComponentList<T>(List<T> destination, ICombatLogDataField data, bool isKeyValuePair)
        where T : CombatLogEventComponent
    {
        if (!componentConstructors.TryGetValue(typeof(T), out var constructor))
        {
            componentConstructors[typeof(T)] = constructor = GetActivator(typeof(T));
        }

        var action = this[typeof(T)] ?? throw new NullReferenceException($"No mapping for type \"{typeof(T).Name}\" could be found.");

        var listData = isKeyValuePair ?
            CollateKeyValuePairs(((CombatLogDataFieldCollection)data).Children) :
            ((CombatLogDataFieldCollection)data).Children;

        foreach (var item in listData)
        {
            var newItem = (T)constructor();
            if (item is CombatLogDataFieldCollection collection)
            {
                action(newItem, collection.Children);
            }
            else
            {
                action(newItem, [item]);
            }
            destination.Add(newItem);
        }
    }

    internal int MapComponentAsProperties(CombatLogEventComponent component, List<ICombatLogDataField> data, int currentIndex)
    {
        return currentIndex < data.Count ? MapComponent(component, data[currentIndex..]) + currentIndex : currentIndex;
    }

    internal int MapComponent(CombatLogEventComponent component, List<ICombatLogDataField> data)
    {
        if (data.Count == 0) return 0;
        var action = this[component.GetType()];
        return action is null
            ? throw new NullReferenceException($"No mapping for type \"{component.GetType().Name}\" could be found.")
            : action(component, data);
    }

    private static List<ICombatLogDataField> CollateKeyValuePairs(List<ICombatLogDataField> data)
    {
        return [.. data
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / 2)
            .Select(x =>
            {
                var group = new CombatLogDataFieldCollection { OpeningBracket = '[', };
                x.Select(x => x.Value).ToList().ForEach(x => group.AddChild(x));
                return (ICombatLogDataField)group;
            })];
    }
}

/// <summary>
/// Maps the data to the specified component.
/// </summary>
/// <param name="component">Instance of a <see cref="CombatLogEventComponent"/></param>
/// <param name="data"></param>
/// <returns>The last accessed index of <paramref name="data"/>.</returns>
internal delegate int CombatLogEventMapping(CombatLogEventComponent component, List<ICombatLogDataField> data, int startIndex = 0);
