using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nwpie.Foundation.DataAccess.Database
{
    [Serializable]
    public class KeyedObjectCollection<TKey, T> : KeyedCollection<TKey, T>
        where T : IKeyedObject<TKey>
    {
        public KeyedObjectCollection() { }

        public KeyedObjectCollection(IEqualityComparer<TKey> comparer)
            : base(comparer) { }

        protected override TKey GetKeyForItem(T item) =>
            item.Key;

        protected override void InsertItem(int index, T item)
        {
            if (Contains(item.Key))
            {
                throw new Exception($"Duplicate Key (={item.Key}). ");
            }

            base.InsertItem(index, item);
        }

        public new T this[TKey key]
        {
            get
            {
                var result = default(T);
                if (Contains(key))
                {
                    result = base[key];
                }

                return result;
            }
        }
    }

    public class KeyedObjectCollection<T> : KeyedObjectCollection<string, T>
        where T : IKeyedObject
    {
        public KeyedObjectCollection()
            : base(StringComparer.OrdinalIgnoreCase) { }

        public KeyedObjectCollection(StringComparer comparer)
            : base(comparer ?? StringComparer.OrdinalIgnoreCase) { }
    }
}
