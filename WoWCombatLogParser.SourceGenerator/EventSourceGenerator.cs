using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.SourceGenerator.Events
{
    [Generator]
    public class EventSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var sections = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(EventSection)) && !x.IsAbstract && !x.IsGenericType);

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
                                var generatedItem = EventSourceGeneratorExtensions.GenerateEventSource<CompoundEventSection>(
                                    types: new[] { affix.EventType, suffix.EventType },
                                    inheritsFrom: new[] { "CombatLogEvent", "ICombatLogEvent", "IActionCombatLogEvent" },
                                    @namespace: "WoWCombatLogParser.Events",
                                    usings: new[] { "WoWCombatLogParser.Models", "WoWCombatLogParser.Sections" });

                                context.AddSource(generatedItem.name, generatedItem.source);
                            });
                    }
                    else
                    {
                        var generatedItem = EventSourceGeneratorExtensions.GenerateEventSource<EventSection>(
                            types: new[] { affix.EventType },
                            inheritsFrom: new[] { "CombatLogEvent", "ICombatLogEvent" },
                            @namespace: "WoWCombatLogParser.Events", 
                            usings: new[] { "WoWCombatLogParser.Models", "WoWCombatLogParser.Sections" });

                        context.AddSource(generatedItem.name, generatedItem.source);
                    }
                });

            sections.Where(x => !events.Any(e => e.EventType == x))
                .ToList()
                .ForEach(s =>
                {
                    var generatedItem = EventSourceGeneratorExtensions.GenerateEventSource(new[] { s }, new[] { "EventSection" }, null, "WoWCombatLogParser.Sections", new[] { "WoWCombatLogParser.Models", "WoWCombatLogParser.Events" }, false);
                    context.AddSource(generatedItem.name, generatedItem.source);
                });
        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
        }
    }

    public static class EventSourceGeneratorExtensions
    {

        public static (string name, SourceText source) GenerateEventSource<T>(Type[] types, string[] inheritsFrom, string @namespace, IList<string> usings, bool generateAdditionalConstructor = true)
        {
            return GenerateEventSource(types, inheritsFrom, typeof(T).GetTypePropertyInfo(), @namespace, usings, generateAdditionalConstructor);
        }

        public static (string name, SourceText source) GenerateEventSource(Type[] types, string[] inheritsFrom, List<PropertyInfo> baseProperties, string @namespace, IList<string> usings, bool generateAdditionalConstructor = true)
        {
            var sb = new StringBuilder();
            var className = string.Join("", types.Select(x => x.Name));
            sb.Append($@"using System;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Events;
{string.Join(Environment.NewLine, usings?.Select(x => $"using {x};"))}

namespace {@namespace}
{{
");
            var affix = string.Join("", types.Where(x => x.GetCustomAttribute<AffixAttribute>() != null).Select(x => x.GetCustomAttribute<AffixAttribute>().Name));
            if (!string.IsNullOrEmpty(affix))
            {
                sb.AppendLine($"\t[Affix(\"{string.Join("", types.Select(x => x.GetCustomAttribute<AffixAttribute>().Name))}\")]");
            }

            sb.AppendLine($"\tpublic class {className}{(inheritsFrom?.Length > 0 ? $" : {string.Join(", ", inheritsFrom)}" : "")} \r\n\t{{");

            sb.AppendLine($@"
        public {className}() : base()
        {{
        }}
");

            if (generateAdditionalConstructor)
            {
                sb.AppendLine($@"
        public {className}(IList<IField> line) : base(line, false)
        {{
        }}
");
            }            

            baseProperties?.ForEach(x => sb.AppendLine(x.GetProperty(2)));
            foreach (var type in types)
            {
                foreach (var prop in type.GetTypePropertyInfo())
                {
                    sb.AppendLine($"{prop.GetProperty(2)}");
                }
            }

            sb.Append("\t}\r\n}");

            return ($"{className}.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        public static List<PropertyInfo> GetTypePropertyInfo(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.GetCustomAttribute<NonDataAttribute>() == null && (i.PropertyType.IsSubclassOf(typeof(EventSection)) || i.CanWrite))
                .OrderBy(i => i.DeclaringType == type)
                .ToList();
            return properties;
        }

        public static string GetProperty(this PropertyInfo property, int indentation = 0)
        {
            string propertyType;
            if (typeof(IEventSectionList).IsAssignableFrom(property.PropertyType))
            {
                var genericParameters = string.Join(",", property.PropertyType.GetGenericArguments().Select(x => x.Name));
                propertyType = $"{property.PropertyType.GetNameWithoutGenericArity()}<{genericParameters}>";
            }
            else
            {
                propertyType = property.PropertyType.Name;
            }

            var value = $"{new string('\t', indentation)}public {propertyType} {property.Name}";
            if (property.CanRead || property.CanWrite)
            {
                value += $" {{{(property.CanRead ? " get;" : "")}{(property.CanWrite ? " set;" : "")} }}";
            }

            if (property.PropertyType.IsSubclassOf(typeof(EventSection)) || typeof(IEventSectionList).IsAssignableFrom(property.PropertyType))
            {
                value += " = new();";
            }

            return value;
        }

        public static string GetNameWithoutGenericArity(this Type t)
        {
            string name = t.Name;
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}
