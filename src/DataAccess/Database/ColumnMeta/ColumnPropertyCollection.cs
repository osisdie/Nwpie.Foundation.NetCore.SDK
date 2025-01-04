using System.Collections.Generic;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class ColumnPropertyCollection : KeyedObjectCollection<ColumnProperty>
    {
        public ColumnPropertyCollection()
        {
            PrimaryKeyColumnProperties = new List<ColumnProperty>();
        }

        protected override void InsertItem(int index, ColumnProperty item)
        {
            base.InsertItem(index, item);

            if (item.IsPrimaryKey)
            {
                PrimaryKeyColumnProperties.Add(item);
            }
        }

        public List<ColumnProperty> PrimaryKeyColumnProperties { get; private set; }
    }
}
