using System;
using System.Collections.ObjectModel;

namespace Nwpie.Foundation.DataAccess.Database
{
    [Serializable]
    public abstract class KeyedCollectionBase<TKey, T> : KeyedCollection<TKey, T>
    {
        protected override void InsertItem(int index, T item)
        {
            var keyForItem = GetKeyForItem(item);
            if (Contains(keyForItem))
            {
                throw new Exception($"Duplicate KeyName (={keyForItem}), TypeName (={GetType().FullName}). ");
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
}
