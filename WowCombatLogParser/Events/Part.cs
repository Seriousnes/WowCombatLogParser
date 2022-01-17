using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Events
{
    public abstract class Part : IEnumerable<PropertyInfo>
    {
        public virtual bool Parse(IEnumerator<string> data)
        {               
            foreach (var property in this)
            {
                if (!ParseProperty(property, data)) return false;
            }
            return true;
        }

        protected virtual bool ParseProperty(PropertyInfo property, IEnumerator<string> data)
        {
            bool parseResult;
            if (property.PropertyType.IsSubclassOf(typeof(Part)))
            {
                var prop = (Part)property.GetValue(this);
                parseResult = prop.Parse(data);
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(PartList<>))
            {
                parseResult = (bool)property.PropertyType.GetMethod("Parse").Invoke(property.GetValue(this), new[] { data });
            }
            else
            {
                parseResult = data.MoveNext() && SetPropertyValue(property, data.Current);
            }
            return parseResult;
        }

        protected virtual bool SetPropertyValue(PropertyInfo property, string value)
        {
            property.SetValue(this, Conversion.GetValue(value, property.PropertyType));
            return true;
        }

        public IEnumerator<PropertyInfo> GetEnumerator()
        {
            return new PartEnumerator(this); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class ComplexPart : Part
    {
        private static readonly Regex _r = new Regex(@"[\(\[]?(.*?)[\)\]]?", RegexOptions.Compiled);
        protected override bool SetPropertyValue(PropertyInfo property, string value)
        {
            var result = base.SetPropertyValue(property, _r.Replace(value, "$1"));
            return result || value.Last().In(']', ')');
        }
    }

    public class PartList<T> : List<T> where T : ComplexPart
    {
        public virtual bool Parse(IEnumerator<string> data)
        {
            do
            {
                T item = EventGenerator.NewPart<T>();
                if (item.Parse(data))
                {
                    Add(item);
                }
            } while (!data.Current.Last().In(']', ')'));
            return true;
        }
    }

    public class PartEnumerator : IEnumerator<PropertyInfo>, IDisposable
    {
        private ClassMap classMap;
        private int position = -1;

        public PartEnumerator(Part part)
        {
            classMap = EventGenerator.GetClassMap(part.GetType());            
        }

        object IEnumerator.Current => Current;
        public PropertyInfo Current
        {
            get
            {
                try
                {
                    return classMap.Properties[position];
                }
                catch
                {
                    throw new InvalidOperationException();
                }                    
            }
        }

        public bool MoveNext()
        {            
            return ++position < classMap.Properties.Length;
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {            
        }
    }
}
