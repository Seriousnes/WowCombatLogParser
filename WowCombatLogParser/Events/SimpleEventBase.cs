using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events
{
    public class EventSection
    {
        public virtual void Parse(IEnumerator<string> enumerator)
        {
            var properties = this.GetType()
                    .GetProperties()
                    .Where(i => Attribute.IsDefined(i, typeof(FieldOrderAttribute)))
                    .OrderBy(i => ((FieldOrderAttribute)Attribute.GetCustomAttribute(i, typeof(FieldOrderAttribute))).Id)
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
                    var method = typeof(Conversions).GetMethod(nameof(Conversions.GetValue));
                    var generic = method.MakeGenericMethod(property.PropertyType);

                    if (enumerator.MoveNext())
                    {
                        property.SetValue(this, generic.Invoke(this, new object[] { enumerator.Current }));
                    }
                }
            }
        }
    }

    public class SimpleEventBase : EventSection
    {
        [FieldOrder(1)]
        public DateTime Timestamp { get; set; }
        [FieldOrder(2)]
        public string Event { get; set; }
    }

    public class ComplexEventBase : EventSection
    {
        [FieldOrder(1)]
        public DateTime Timestamp { get; set; }
        [FieldOrder(2)]
        public string Event { get; set; }
        
        [FieldOrder(3)]
        public Unit Source { get; } = new Unit();
        [FieldOrder(4)]
        public Unit Destination { get; } = new Unit();        
    }
}
