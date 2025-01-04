using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Nwpie.Foundation.DataAccess.Database
{
    public static class IEnumerableExtensions
    {
        public static SqlMapper.ICustomQueryParameter AsTableValuedParameter<T>(this IEnumerable<T> enumerable, string typeName)
        {
            var dataTable = new DataTable();
            if (typeof(T).IsValueType || typeof(T).FullName.Equals("System.String"))
            {
                dataTable.Columns.Add("NONAME");
                foreach (var obj in enumerable)
                {
                    dataTable.Rows.Add(obj);
                }
            }
            else
            {
                var properties = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertie in properties)
                {
                    dataTable.Columns.Add(propertie.Name, propertie.PropertyType);
                }

                foreach (var obj in enumerable)
                {
                    dataTable.Rows.Add(properties.Select(o => o.GetValue(obj)).ToArray());
                }
            }

            return dataTable.AsTableValuedParameter(typeName);
        }
    }
}
