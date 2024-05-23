using ExpressionDebugger;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;
using WoWCombatLogParser.Parser.EventMapping;
using static System.Linq.Expressions.Expression;

namespace WoWCombatLogParser;

/// <summary>
/// Maps the data to the specified component.
/// </summary>
/// <param name="component">Instance of a <see cref="CombatLogEventComponent"/></param>
/// <param name="data"></param>
/// <returns>The last accessed index of <paramref name="data"/>.</returns>
public delegate int CombatLogEventMapping(CombatLogEventComponent component, List<ICombatLogDataField> data, int startIndex = 0);

public class CombatLogEventMapper : ICombatLogEventMapper
{
    private readonly Dictionary<Type, CombatLogEventMapping> maps;
    private readonly Dictionary<Type, ObjectActivator> constructors = [];

    [DisallowNull]
    public CombatLogEventMapping? this[Type index]
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

    public CombatLogEventMapper()
    {
        // add any custom mapping overrides
        maps = GetType().Assembly
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(EventProfile)))
            .Select(x => (EventProfile)Activator.CreateInstance(x)!)
            .ToDictionary(key => key.EventType, value => value.GetMapping(this));        
    }

    private void Setup()
    {
        var types = GetTypesWhere(i =>
            i.GetCustomAttribute<AffixAttribute>() == null &&
            i.IsSubclassOf(typeof(CombatLogEventComponent)) &&
            !i.IsAbstract &&
            !i.IsGenericType);

        foreach (var t in types)
        {
            BuildDelegateForType(t);
        }
    }

    public CombatLogEventMapping? BuildDelegateForType(Type type)
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
                        nameof(CombatLogEventMapper.MapListItems),
                        [
                            member.Type.GetGenericArguments()[0],
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
                                typeof(CombatLogEventMapper).GetMethod(
                                    nameof(CombatLogEventMapper.MapFlatProperty),
                                    BindingFlags.Instance | BindingFlags.NonPublic
                                )!,
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
                                typeof(CombatLogEventMapper).GetMethod(
                                    nameof(CombatLogEventMapper.MapFlatProperty),
                                    BindingFlags.Instance | BindingFlags.NonPublic                                    
                                )!,
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
                                typeof(Conversion).GetMethod(nameof(Conversion.GetValue), [typeof(ICombatLogDataField), typeof(Type)])!,
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

    internal void MapListItems<T>(List<T> destination, ICombatLogDataField data, bool isKeyValuePair)
        where T : CombatLogEventComponent
    {
        if (!constructors.TryGetValue(typeof(T), out var constructor))
        {
            constructor = CombatLogEventActivator.GetActivator(typeof(T));
            constructors[typeof(T)] = constructor;
        }
        
        var action = this[typeof(T)] ?? throw new NullReferenceException($"No mapping for type \"{typeof(T).Name}\" could be found.");

        var listData = ((CombatLogDataFieldCollection)data).Children;
        if (isKeyValuePair)
            listData = CollateKeyValuePairs(listData);
        
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

    internal int MapFlatProperty(CombatLogEventComponent component, List<ICombatLogDataField> data, int currentIndex)
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

    internal static List<ICombatLogDataField> CollateKeyValuePairs(List<ICombatLogDataField> data)
    {
        return data
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / 2)
            .Select(x =>
            {
                var group = new CombatLogDataFieldCollection { OpeningBracket = '[', };
                x.Select(x => x.Value).ToList().ForEach(x => group.AddChild(x));
                return (ICombatLogDataField)group;
            })
            .ToList();
    }

    public static Expression For(ParameterExpression loopVar, Expression list, Expression body)
    {
        var i_Assign = Assign(loopVar, Constant(0));
        
        var count = Variable(typeof(int));
        var assignCount = Assign(count, MakeMemberAccess(list, list.Type.GetMember("Count")[0]));

        var increment = PostIncrementAssign(loopVar);
        var _break = Label("Break");
        var loop = Loop(
            IfThenElse(
                LessThan(loopVar, count),
                Block(
                    typeof(void),
                    [loopVar],
                    body,
                    increment
                ),
                Break(_break)
            ),
            _break
        );
        return Block(
            typeof(void),
            [loopVar, count],
            i_Assign,
            assignCount,
            loop
        );
    }

    public static Expression ForEach(Expression enumerable, ParameterExpression loopVar, Expression loopContent)
    {
        var elementType = loopVar.Type;
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
        var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

        var enumeratorVar = Variable(enumeratorType, "enumerator");
        var getEnumeratorCall = Call(enumerable, enumerableType.GetMethod("GetEnumerator")!);
        var enumeratorAssign = Assign(enumeratorVar, getEnumeratorCall);
        var enumeratorDispose = Call(enumeratorVar, typeof(IDisposable).GetMethod("Dispose")!);

        // The MoveNext method's actually on IEnumerator, not IEnumerator<T>
        var moveNextCall = Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext")!);

        var breakLabel = Label("LoopBreak");

        var trueConstant = Constant(true);

        var loop =
            Loop(
                IfThenElse(
                    Equal(moveNextCall, trueConstant),
                    Block(
                        typeof(void),
                        [loopVar],
                        Assign(loopVar, Property(enumeratorVar, "Current")),
                        loopContent),
                    Break(breakLabel)),
                breakLabel);

        var tryFinally =
            TryFinally(
                loop,
                enumeratorDispose);

        var body =
            Block(
                [enumeratorVar],
                enumeratorAssign,
                tryFinally);

        return body;
    }

    private static IEnumerable<Type> GetTypesWhere(Func<Type, bool> expr)
    {
        foreach (var type in Assembly.Load("WoWCombatLogParser.Common").GetTypes().Where(expr))
            yield return type;
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(expr))
            yield return type;
    }
}

public interface ICombatLogEventMapper
{
    CombatLogEventMapping? this[Type type] { get; }
}