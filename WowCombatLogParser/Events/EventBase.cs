using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utilities;

namespace WoWCombatLogParser.Events
{
    public class EventSection
    {
        public virtual void Parse(IEnumerator<string> enumerator)
        {
            var properties = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(i => i.DeclaringType == this.GetType())
                .ToList();            
            ParsePropeties(enumerator, properties);
        }

        private void ParsePropeties(IEnumerator<string> enumerator, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (property.PropertyType.IsSubclassOf(typeof(EventSection)))
                {
                    var prop = (EventSection)property.GetValue(this);
                    prop.Parse(enumerator);
                }
                else
                {
                    if (!property.CanWrite) continue;
                    var method = typeof(Conversions).GetMethod(nameof(Conversions.GetValue));
                    var generic = method.MakeGenericMethod(property.PropertyType);
                    var columnsToSkip = property.GetCustomAttribute<OffsetAttribute>()?.Value ?? 0;

                    if (enumerator.MoveBy(columnsToSkip))
                    {
                        try
                        {
                            property.SetValue(this, generic.Invoke(this, new object[] { enumerator.Current }));
                        }
                        catch (Exception)
                        {
                            throw new CombatLogParseException($"{this.GetType().FullName}.{property.Name}", property.GetType(), enumerator.Current);
                        }
                    }
                }
            }
        }
    }

    public class EventBase : EventSection
    {
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }
    }

    [DebuggerDisplay("{Event} @ {Timestamp} {Source} > {Destination}")]
    public class ComplexEventBase : EventBase
    {
        public Actor Source { get; } = new();
        public Actor Destination { get; } = new();        
    }
}
