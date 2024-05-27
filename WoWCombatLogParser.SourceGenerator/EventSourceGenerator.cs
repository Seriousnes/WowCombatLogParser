using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WoWCombatLogParser.SourceGenerator.Events.Compound;
using WoWCombatLogParser.SourceGenerator.Events.Compound.Predefined;
using WoWCombatLogParser.SourceGenerator.Models;
using static WoWCombatLogParser.SourceGenerator.EventSourceGeneratorExtensions;

namespace WoWCombatLogParser.SourceGenerator;

[Generator]
public class EventSourceGenerator : ISourceGenerator
{
    private readonly static Dictionary<Type, IList<string>> defaultInheritance = new()
    {
        { typeof(CompoundEventSection), new[] { "CombatLogEvent", "ICombatLogEvent", "IAction" }},
        { typeof(CombatLogEventComponent), new[] { "CombatLogEvent", "ICombatLogEvent" }}
    };

    public void Execute(GeneratorExecutionContext context)
    {
        //var sections = Assembly.GetExecutingAssembly()
        //    .GetTypes()
        //    .Where(x => x.IsSubclassOf(typeof(CombatLogEventComponent)) && !x.IsAbstract/* && !x.IsGenericType*/);

        var events = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.GetCustomAttribute<AffixAttribute>() != null)
            .ToList()
            .ConvertAll(x => new EventAffixItem(x));

        var suffixEvents = events.Where(affix => affix.IsSuffix);
        events.Where(affix => !affix.IsSuffix)
            .ToList()
            .ForEach(affix =>
            {
                if (affix.IsPrefix)
                {
                    var suffixes = affix.HasRestrictedSuffixes ? affix.RestrictedSuffixes : suffixEvents;
                    suffixes.Where(suffix => !affix.CheckSuffixIsAllowed(suffix.EventType))
                        .ToList()
                        .ForEach(suffix =>
                        {
                            var (name, source) = GenerateSourceText<CompoundEventSection>(
                                    types: [affix.EventType, suffix.EventType],
                                    @namespace: "WoWCombatLogParser",
                                    usings: []);

                            AddSource(context, name, source);
                        });
                }
                else
                {
                    (string name, SourceText source) generatedItem;
                    if (affix.EventType.IsSubclassOf(typeof(PredefinedBase)))
                    {
                        generatedItem = GenerateSourceText<CompoundEventSection>(
                                    types: [affix.EventType],
                                    @namespace: "WoWCombatLogParser",
                                    usings: []);
                    }
                    else
                    {
                        generatedItem = GenerateSourceText<CombatLogEventComponent>(
                            types: [affix.EventType],
                            @namespace: "WoWCombatLogParser",
                            usings: []);
                    }

                    AddSource(context, generatedItem.name, generatedItem.source);
                }
            });
    }

    private void AddSource(GeneratorExecutionContext context, string fileName, SourceText content) => AddSource(context, fileName, content.ToString());
    private void AddSource(GeneratorExecutionContext context, string fileName, string content)
    {
        context.AddSource(
            fileName,
            CSharpSyntaxTree.ParseText(
                content,
                new(LanguageVersion.Latest, DocumentationMode.Diagnose))
            .GetRoot()
            .NormalizeWhitespace()
            .ToFullString());
    }

    public void Initialize(GeneratorInitializationContext context)
    {
//#if DEBUG
//        if (!Debugger.IsAttached)
//        {
//            Debugger.Launch();
//        }
//#endif
    }

    private (string name, SourceText source) GenerateSourceText(IList<Type> types, IList<PropertyInfo> baseProperties, string @namespace, IList<string> usings, IList<string> inheritsFrom = null, bool generateAdditionalConstructor = true)
    {
        inheritsFrom ??= defaultInheritance.TryGetValue(null, out var values) ? values : [];
        var className = string.Join("", types.Select(x => x.Name));
        return ($"{className}.cs", SourceText.From($@"{GetUsings([.. usings])}

namespace {@namespace}
{{
    {GetClassData(className, types, inheritsFrom, baseProperties, generateAdditionalConstructor)}
}}", Encoding.UTF8));
    }

    private (string name, SourceText source) GenerateSourceText<T>(Type[] types, string @namespace = null, IList<string> usings = null, bool generateAdditionalConstructor = true) where T : CombatLogEventComponent
    {
        return GenerateSourceText(types, typeof(T).GetTypePropertyInfo(), @namespace, usings, defaultInheritance.TryGetValue(typeof(T), out var inheritsFrom) ? inheritsFrom : null, generateAdditionalConstructor);
    }

    private string GetInheritance(IList<Type> types, IList<string> predefined)
    {
        predefined ??= [];
        List<string> inheritance =
        [
            .. types.SelectMany(x => x.GetInterfaces().Select(i => i.Name)),
            .. types.Where(x => x.BaseType?.IsGenericType ?? false).SelectMany(x => x.BaseType?.GetGenericArguments().Where(x => !x.IsSealed).SelectMany(g => g.GetInterfaces())?.Select(i => i.Name)),
        ];

        var value = string.Join(", ", predefined.Union(inheritance.Distinct()));
        return !string.IsNullOrEmpty(value) ? $" : {value}" : "";
    }

    private string GetUsings(params string[] usings)
    {
        return $@"using System;
{string.Join(Environment.NewLine, usings?.Select(x => $"using {x};"))}";
    }

    private string GetAffix(IList<Type> types)
    {
        var affix = string.Join("", types.Where(x => x.GetCustomAttribute<AffixAttribute>() != null).Select(x => x.GetCustomAttribute<AffixAttribute>().Name));
        if (!string.IsNullOrEmpty(affix))
            return $"[Discriminator(\"{string.Join("", types.Select(x => x.GetCustomAttribute<AffixAttribute>().Name))}\")]";
        return "";
    }

    private string GetApplicableCombatLogVersion(IList<Type> types)
    {
        var versions = types.SelectMany(x => x.GetCustomAttributes<CombatLogVersionAttribute>()).Select(x => x.Value) ?? [CombatLogVersion.Any];
        return string.Join(Environment.NewLine, versions.Select(x => $"[CombatLogVersion(CombatLogVersion.{x})]"));
    }

    private string GetDataFieldAttributes(IList<Type> types)
    {
        if (types.Any(x => x.GetCustomAttribute<IsSingleDataFieldAttribute>() != null))
        {
            return "[IsSingleDataField]";
        }
        return string.Empty;
    }

    private string GetClassData(string className, IList<Type> types, IList<string> inheritsFrom, IList<PropertyInfo> baseProperties, bool generateAdditionalConstructor)
    {
        return 
$$"""
{{ConsolidateAttributes(
    GetAffix(types),
    GetApplicableCombatLogVersion(types),
    GetDataFieldAttributes(types))}}
    public partial class {{className}}{{GetInheritance(types, inheritsFrom)}}
    {
        public {{className}}() : base()
        {
        }
        
        {{(baseProperties?.Count > 0 ? string.Join(Environment.NewLine, baseProperties.ToList().Select(x => x.GetProperty())) : "")}}
        {{GetProperties(types)}}
    }
""";
    }

    private string GetProperties(IList<Type> types)
    {
        var sb = new StringBuilder();
        var predefinedBaseTypes = new List<Type>();
        if (types.Count == 1 && types[0].IsSubclassOf(typeof(PredefinedBase)))
        {
            predefinedBaseTypes.AddRange(types[0].BaseType.GenericTypeArguments);
        }

        types = [.. predefinedBaseTypes, .. types];

        foreach (var type in types)
        {
            foreach (var prop in type.GetTypePropertyInfo())
            {
                sb.AppendLine($"{prop.GetProperty()}");
            }
        }

        return sb.ToString();
    }
}

public static class EventSourceGeneratorExtensions
{
    public static string GetProperty(this PropertyInfo property)
    {
        string propertyType;
        if (property.PropertyType.GenericTypeArguments.Any())
        {
            var genericParameters = string.Join(",", property.PropertyType.GetGenericArguments().Select(x => x.Name));
            propertyType = $"{property.PropertyType.GetNameWithoutGenericArity()}<{genericParameters}>";
        }
        else
        {
            propertyType = property.PropertyType.Name;
        }

        var isReferencType = !property.PropertyType.IsValueType;

        var attributes = new List<string>();
        foreach (var attribute in property.CustomAttributes.Select(x => x.AttributeType))
        {
            var attributeName = Regex.Replace(attribute.Name, @"^(.*?)Attribute$", @"$1");
            attributes.Add($"[{attributeName}]");
        }
        var value = @$"{ConsolidateAttributes([.. attributes])}
        public {propertyType}{(isReferencType ? "?" : "")} {property.Name}";

        if (property.CanRead || property.CanWrite)
        {
            value += $" {{{(property.CanRead ? " get;" : "")}{(property.CanWrite ? " set;" : "")} }}";
        }

        if (property.PropertyType.IsSubclassOf(typeof(CombatLogEventComponent)) || property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            value += " = new();";
        }

        return value;
    }

    public static string ConsolidateAttributes(params string[] attributes)
    {
        return string.Join($"{Environment.NewLine}", attributes.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => $"{a}"));
    }

    public static string GetNameWithoutGenericArity(this Type t)
    {
        string name = t.Name;
        int index = name.IndexOf('`');
        return index == -1 ? name : name.Substring(0, index);
    }
}
