
namespace Nwpie.Foundation.DataAccess.Database
{
    public class ColumnMetaInfoCollection : KeyedCollectionBase<string, ColumnMetaInfo>
    {
        protected override string GetKeyForItem(ColumnMetaInfo item) =>
            item.ColumnName;
    }
}
