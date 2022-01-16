using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Events
{
    public abstract class Part
    {
        protected virtual void Parse(IEnumerator<string> data)
        {
            foreach (var property in EventGenerator.GetClassMap(GetType()))
            {
                if (property.PropertyType.IsSubclassOf(typeof(Part)))
                {
                    var prop = (Part)property.GetValue(this);
                    prop.Parse(data);
                }
                else
                {
                    if (!property.CanWrite) continue;
                    if (data.MoveNext())
                    {
                        property.SetValue(this, Conversion.GetValue(data.Current, property.PropertyType));
                    }
                }                
            }
        }
    }
}
