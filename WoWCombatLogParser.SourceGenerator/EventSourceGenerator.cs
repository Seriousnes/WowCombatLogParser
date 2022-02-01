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
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.SourceGenerator.Events
{
    [Generator]
    public class EventSourceGenerator : ISourceGenerator
    {
        private static Dictionary<Type, IList<string>> defaultInheritance = new Dictionary<Type, IList<string>>
        {
            { typeof(CompoundEventSection), new[] { "CombatLogEvent", "ICombatLogEvent", "IAction" }},
            { typeof(EventSection), new[] { "CombatLogEvent", "ICombatLogEvent" }}
        };

        public void Execute(GeneratorExecutionContext context)
        {
            var sections = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(EventSection)) && !x.IsAbstract/* && !x.IsGenericType*/);

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
                                var generatedItem = GenerateSourceText<CompoundEventSection>(
                                        types: new[] { affix.EventType, suffix.EventType },
                                        @namespace: "WoWCombatLogParser.Events",
                                        usings: new[] { "WoWCombatLogParser.Models", "WoWCombatLogParser.Sections" });

                                context.AddSource(generatedItem.name, generatedItem.source);
                            });
                    }
                    else
                    {
                        (string name, SourceText source) generatedItem;
                        if (affix.EventType.IsSubclassOf(typeof(PredefinedBase)))
                        {
                            generatedItem = GenerateSourceText<CompoundEventSection>(
                                        types: new[] { affix.EventType },
                                        @namespace: "WoWCombatLogParser.Events",
                                        usings: new[] { "WoWCombatLogParser.Models", "WoWCombatLogParser.Sections" });
                        }
                        else
                        {
                            generatedItem = GenerateSourceText<EventSection>(
                                types: new[] { affix.EventType },
                                @namespace: "WoWCombatLogParser.Events",
                                usings: new[] { "WoWCombatLogParser.Models", "WoWCombatLogParser.Sections" });
                        }

                        context.AddSource(generatedItem.name, generatedItem.source);
                    }
                });

            sections.Where(x => !events.Any(e => e.EventType == x))
                .ToList()
                .ForEach(s =>
                {
                    var generatedItem = GenerateSourceText(new[] { s }, null, "WoWCombatLogParser.Sections", new[] { "WoWCombatLogParser.Models", "WoWCombatLogParser.Events" }, new[] { "EventSection" }, false);
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

        private (string name, SourceText source) GenerateSourceText(IList<Type> types, IList<PropertyInfo> baseProperties, string @namespace, IList<string> usings, IList<string> inheritsFrom = null, bool generateAdditionalConstructor = true)
        {
            if (inheritsFrom == null)
                inheritsFrom = defaultInheritance.TryGetValue(null, out var values) ? values : new List<string>();
            var className = string.Join("", types.Select(x => x.Name));
            return ($"{className}.cs", SourceText.From($@"{GetUsings(usings.ToArray())}

namespace {@namespace}
{{
    {GetClassData(className, types, inheritsFrom, baseProperties, generateAdditionalConstructor)}
}}", Encoding.UTF8));
        }

        private (string name, SourceText source) GenerateSourceText<T>(Type[] types, string @namespace, IList<string> usings, bool generateAdditionalConstructor = true) where T : EventSection
        {
            return GenerateSourceText(types, typeof(T).GetTypePropertyInfo(), @namespace, usings, defaultInheritance.TryGetValue(typeof(T), out var inheritsFrom) ? inheritsFrom : null, generateAdditionalConstructor);
        }

        private string GetInheritance(IList<Type> types, IList<string> predefined)
        {        
            if (predefined == null)
                predefined = new List<string>();
            List<string> inheritance = new List<string>();

            inheritance.AddRange(types.SelectMany(x => x.GetInterfaces().Select(i => i.Name)));            
            inheritance.AddRange(types.Where(x => x.BaseType?.IsGenericType ?? false).SelectMany(x => x.BaseType?.GetGenericArguments().SelectMany(g => g.GetInterfaces())?.Select(i => i.Name)));

            var value = string.Join(", ", predefined.Union(inheritance.Distinct()));
            return !string.IsNullOrEmpty(value) ? $" : {value}" : "";
        }

        private string GetUsings(params string[] usings)
        {
            return $@"using System;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Events;
{string.Join(Environment.NewLine, usings?.Select(x => $"using {x};"))}";
        }

        private string GetAffix(IList<Type> types)
        {
            var affix = string.Join("", types.Where(x => x.GetCustomAttribute<AffixAttribute>() != null).Select(x => x.GetCustomAttribute<AffixAttribute>().Name));
            if (!string.IsNullOrEmpty(affix))
                return $"[Affix(\"{string.Join("", types.Select(x => x.GetCustomAttribute<AffixAttribute>().Name))}\")]";
            return "";
        }

        private string GetClassData(string className, IList<Type> types, IList<string> inheritsFrom, IList<PropertyInfo> baseProperties, bool generateAdditionalConstructor)
        {
            return $@"{GetAffix(types)}
    public class {className}{GetInheritance(types, inheritsFrom)}
    {{
        public {className}() : base()
        {{
        }}
{GetAdditionalConstructor(className, generateAdditionalConstructor)}        

{(baseProperties?.Count > 0 ? string.Join(Environment.NewLine, baseProperties.ToList().Select(x => x.GetProperty(2))) : "")}
{GetProperties(types)}
    }}";
        }

        private string GetAdditionalConstructor(string className, bool generate)
        {
            return generate ? $@"
        public {className}(DateTime timestamp, string @event, string data) : base(timestamp, @event, data)
        {{
        }}" : "";
        }

        private string GetProperties(IList<Type> types)
        {
            var sb = new StringBuilder();
            if (types.Count == 1 && types[0].IsSubclassOf(typeof(PredefinedBase)))
            {
                types = types[0].BaseType.GenericTypeArguments;
            }

            foreach (var type in types)
            {
                foreach (var prop in type.GetTypePropertyInfo())
                {
                    sb.AppendLine($"{prop.GetProperty(2)}");
                }
            }

            return sb.ToString();
        }
    }

    public static class EventSourceGeneratorExtensions
    {        
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

            var value = $"{new string(' ', 4 * indentation)}public {propertyType} {property.Name}";
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
