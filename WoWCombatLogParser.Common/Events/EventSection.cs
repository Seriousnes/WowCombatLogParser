using System.Collections.Generic;
using System.Reflection;
using WoWCombatLogParser.Common.Utility;
using System.Diagnostics;

namespace WoWCombatLogParser.Common.Events
{
    public interface IEventSection
    {
        bool Parse(IEnumerator<IField> data);
    }

    public abstract class EventSection : IEventSection
    {
        public virtual bool Parse(IEnumerator<IField> data)
        {
            foreach (var property in EventGenerator.GetClassMap(this.GetType()).Properties)
            {
                if (!ParseProperty(property, data)) return false;
            }
            return true;
        }

        protected virtual bool ParseProperty(PropertyInfo property, IEnumerator<IField> data)
        {
            bool parseResult;
            if (property.PropertyType.IsSubclassOf(typeof(EventSection)))
            {
                var (Success, Enumerator, EndOfParent, Dispose) = data.GetEnumeratorForProperty();
                parseResult = Success && ((EventSection)property.GetValue(this)).Parse(Enumerator);

                if (Dispose) Enumerator.Dispose();

                parseResult = parseResult && !EndOfParent;
            }
            else if (typeof(IEventSectionList).IsAssignableFrom(property.PropertyType))
            {
                parseResult = ((IEventSectionList)property.GetValue(this)).Parse(data);
            }
            else
            {
                parseResult = SetPropertyValue(property, data);
            }
            return parseResult;
        }

        protected virtual bool SetPropertyValue(PropertyInfo property, IEnumerator<IField> data)
        {
            property.SetValue(this, Conversion.GetValue(data.Current, property.PropertyType));
            return data.MoveNext();
        }
    }

    public interface IEventSectionList
    {
        bool Parse(IEnumerator<IField> data);
    }

    public class EventSections<T> : List<T>, IEventSectionList where T : EventSection
    {
        public virtual bool Parse(IEnumerator<IField> data)
        {
            var enumeratorResult = data.GetEnumeratorForProperty();

            if (enumeratorResult.Success)
            {
                bool notDone;
                do
                {
                    T item = EventGenerator.CreateEventSection<T>();
                    notDone = item.Parse(enumeratorResult.Enumerator);
                    Add(item);
                } while (notDone);
            }

            if (enumeratorResult.Dispose)
                enumeratorResult.Enumerator.Dispose();

            return true;
        }
    }

    public class NestedEventSections<T> : EventSections<T>, IEventSectionList where T : EventSection
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
                        T item = EventGenerator.CreateEventSection<T>();
                        notDone = item.Parse(innerEnumerator.Enumerator);
                        Add(item);
                    }

                    notDone = !innerEnumerator.EndOfParent || notDone;

                    if (innerEnumerator.Dispose && innerEnumerator.Enumerator != outerEnumerator.Enumerator)
                        innerEnumerator.Enumerator.Dispose();

                } while (notDone);
            }

            return true;
        }
    }

    [DebuggerDisplay("{Id}")]
    public abstract class IdPart : EventSection
    {
        public int Id { get; set; }
    }
}
