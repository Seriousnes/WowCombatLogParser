using System.Collections.Generic;
using System.Reflection;
using WoWCombatLogParser.Common.Utility;
using System;
using WoWCombatLogParser.Common.Models;
using System.Linq;
using System.Collections.Concurrent;

namespace WoWCombatLogParser.Common.Events
{
    public interface IEventSection
    {
        bool Parse(FightDataDictionary fightDataDictionary, IEnumerator<IField> data);
    }

    public abstract class EventSection : IEventSection
    {
        private static readonly Type eventSectionType = typeof(EventSection);
        
        public virtual bool Parse(FightDataDictionary fightDataDictionary, IEnumerator<IField> data)
        {
            foreach (var property in EventGenerator.GetClassMap(this.GetType()).Properties)
            {
                if (!ParseProperty(fightDataDictionary, property, data)) return false;
            }
            return true;
        }

        protected virtual bool ParseProperty(FightDataDictionary fightDataDictionary, PropertyInfo property, IEnumerator<IField> data)
        {
            bool parseResult;
            if (property.PropertyType.IsSubclassOf(eventSectionType))
            {
                if (typeof(IKey).IsAssignableFrom(property.PropertyType) && fightDataDictionary is not null)
                {
                    if (ParseCommonDataProperty(fightDataDictionary, property, data))
                        return true;
                }

                var (Success, Enumerator, EndOfParent, Dispose) = data.GetEnumeratorForProperty();
                parseResult = Success && ((EventSection)property.GetValue(this)).Parse(fightDataDictionary, Enumerator);

                if (Dispose) Enumerator.Dispose();

                parseResult = parseResult && !EndOfParent;
            }
            else if (typeof(IEventSectionList).IsAssignableFrom(property.PropertyType))
            {
                parseResult = ((IEventSectionList)property.GetValue(this)).Parse(fightDataDictionary, data);
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

        protected virtual bool ParseCommonDataProperty(FightDataDictionary fightDataDictionary, PropertyInfo property, IEnumerator<IField> data)
        {
            // get list of current data type
            var _list = fightDataDictionary.TryGetValue(property.PropertyType, out var x) ? x : null;
            if (_list is null)
            {
                _list = new List<IKey>();
                fightDataDictionary.TryAdd(property.PropertyType, _list);
            }

            // if item exists in the list, update reference and skip over defined number of steps
            var indexProperty = EventGenerator.GetClassMap(property.PropertyType)
                .Properties
                .Where(x => x.GetCustomAttribute<KeyAttribute>() != null)
                .SingleOrDefault();

            var indexObj = (EventSection)property.GetValue(this) as IKey;
            indexProperty.SetValue(indexObj, Conversion.GetValue(data.Current, indexProperty.PropertyType));

            lock (_list)
            {
                if (_list.Any(x => x.EqualsKey(indexObj)))
                {
                    property.SetValue(this, _list.Single(x => x.EqualsKey(indexObj)));
                    data.MoveBy(indexProperty.GetCustomAttribute<KeyAttribute>().Fields);
                    return true;
                }
                else
                {
                    _list.Add(indexObj);
                }
            }
            return false;
        }        
    }

    public interface IEventSectionList
    {
        bool Parse(FightDataDictionary fightDataDictionary, IEnumerator<IField> data);
    }

    public class EventSections<T> : List<T>, IEventSectionList where T : EventSection
    {
        public virtual bool Parse(FightDataDictionary fightDataDictionary, IEnumerator<IField> data)
        {
            var enumeratorResult = data.GetEnumeratorForProperty();

            if (enumeratorResult.Success)
            {
                bool _continue;
                do
                {
                    T item = EventGenerator.CreateEventSection<T>();
                    _continue = item.Parse(fightDataDictionary, enumeratorResult.Enumerator);
                    Add(item);
                } while (_continue);
            }

            if (enumeratorResult.Dispose)
                enumeratorResult.Enumerator.Dispose();

            return true;
        }
    }

    public class NestedEventSections<T> : EventSections<T>, IEventSectionList where T : EventSection
    {
        public override bool Parse(FightDataDictionary fightDataDictionary, IEnumerator<IField> data)
        {
            var outerEnumerator = data.GetEnumeratorForProperty();
            if (outerEnumerator.Success)
            {
                bool _continue = false;
                do
                {
                    var innerEnumerator = outerEnumerator.Enumerator.GetEnumeratorForProperty();
                    if (innerEnumerator.Success)
                    {
                        T item = EventGenerator.CreateEventSection<T>();
                        _continue = item.Parse(fightDataDictionary, innerEnumerator.Enumerator);
                        Add(item);
                    }

                    _continue = !innerEnumerator.EndOfParent || _continue;

                    if (innerEnumerator.Dispose && innerEnumerator.Enumerator != outerEnumerator.Enumerator)
                        innerEnumerator.Enumerator.Dispose();

                } while (_continue);
            }

            return true;
        }
    }
}
