namespace Nwpie.Foundation.DataAccess.Database
{
    public class QueryConditionCollection : KeyedObjectCollection<QueryCondition>
    {
        protected override void InsertItem(int index, QueryCondition item)
        {
            item.ParameterName = string.Format("{0}{1}", item.ColumnName, index);
            base.InsertItem(index, item);
        }
    }
}
