using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using WoWCombatLogParser.IO;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Events
{
    public abstract class Part : IEnumerable<PropertyInfo>
    {
        public virtual bool Parse(IEnumerator<IField> data)
        {               
            foreach (var property in this)
            {
                if (!ParseProperty(property, data)) return false;                                
            }
            return true;
        }

        protected virtual bool ParseProperty(PropertyInfo property, IEnumerator<IField> data)
        {
            bool parseResult;
            if (property.PropertyType.IsSubclassOf(typeof(Part)))
            {
                //return ((Part)property.GetValue(this)).Parse(data);

                var enumeratorResult = data.GetEnumeratorForProperty();
                parseResult = enumeratorResult.Success ? ((Part)property.GetValue(this)).Parse(enumeratorResult.Enumerator) : false;

                if (enumeratorResult.Dispose)
                    enumeratorResult.Enumerator.Dispose();

                parseResult = parseResult && !enumeratorResult.EndOfParent;
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().In(typeof(PartList<>), typeof(NestedPartList<>)))
            {
                parseResult = (bool)property.PropertyType.GetMethod("Parse").Invoke(property.GetValue(this), new[] { data });

                //var enumeratorResult = data.GetEnumeratorForProperty();
                //parseResult = enumeratorResult.Success ? (bool)property.PropertyType.GetMethod("Parse").Invoke(property.GetValue(this), new[] { enumeratorResult.Enumerator }) : true;

                //if (enumeratorResult.Dispose) 
                //    enumeratorResult.Enumerator.Dispose();

                //parseResult = parseResult && !enumeratorResult.EndOfParent;
            }
            else
            {
                parseResult = SetPropertyValue(property, data);
                //var enumeratorResult = GetEnumeratorForProperty(data);
                //parseResult = enumeratorResult.Success ? SetPropertyValue(property, enumeratorResult.Enumerator) : true;

                //if (enumeratorResult.Dispose)
                //    enumeratorResult.Enumerator.Dispose();

                //parseResult = parseResult && !enumeratorResult.EndOfParent;
            }
            return parseResult;
        }

        protected virtual bool SetPropertyValue(PropertyInfo property, IEnumerator<IField> data)
        {
            property.SetValue(this, Conversion.GetValue(data.Current.AsString(), property.PropertyType));
            return data.MoveNext();
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

    public class PartList<T> : List<T> where T : Part
    {
        public virtual bool Parse(IEnumerator<IField> data)
        {
            var enumeratorResult = data.GetEnumeratorForProperty();

            if (enumeratorResult.Success)
            {
                bool notDone;
                do
                {
                    T item = EventGenerator.NewPart<T>();
                    notDone = item.Parse(enumeratorResult.Enumerator);
                    Add(item);
                } while (notDone);
            }

            if (enumeratorResult.Dispose)
                enumeratorResult.Enumerator.Dispose();
            
            return true;
        }
    }

    public class NestedPartList<T> : PartList<T> where T : Part
    {
        public override bool Parse(IEnumerator<IField> data)
        {
            var outerEnumerator = data.GetEnumeratorForProperty();
            if (outerEnumerator.Success)
            {
                bool notDone = false;
                do
                {
                    var innerEnumerator = outerEnumerator.Enumerator.GetEnumeratorForProperty();
                    if (innerEnumerator.Success)
                    {
                        T item = EventGenerator.NewPart<T>();
                        notDone = item.Parse(innerEnumerator.Enumerator);
                        Add(item);
                    }

                    notDone = !innerEnumerator.EndOfParent || notDone;

                    if (innerEnumerator.Dispose && innerEnumerator.Enumerator != outerEnumerator.Enumerator)
                        innerEnumerator.Enumerator.Dispose();

                } while (notDone);
            }

            return true;

            //bool notDone;
            //do
            //{
            //    var enumeratorResult = data.GetEnumeratorForProperty();

            //    if (enumeratorResult.Success)
            //    {
            //        T item = EventGenerator.NewPart<T>();
            //        notDone = item.Parse(enumeratorResult.Enumerator);
            //        Add(item);
            //    }
            //    else
            //    {
            //        notDone = false;
            //    }

            //    if (enumeratorResult.Dispose)
            //        enumeratorResult.Enumerator.Dispose();

            //} while (notDone);

            //return true;
        }
    }

    public class PartEnumerator : IEnumerator<PropertyInfo>, IDisposable
    {
        private readonly ClassMap classMap;
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
