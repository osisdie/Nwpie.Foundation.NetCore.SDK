using System.Collections.Generic;
using System.Reflection;

namespace Nwpie.Foundation.DataAccess.Database
{
    public static class EntityHelper
    {
        public static List<FlexibleParameter> ConvertEntityToPropertieList<TEntity>(TEntity entity)
            where TEntity : class
        {
            var list = new List<FlexibleParameter>();
            var typeInfo = typeof(TEntity);
            var properties = typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertie in properties)
            {
                var parameter = new FlexibleParameter
                {
                    Name = propertie.Name,
                    Value = propertie.GetValue(entity),
                    Enable = true,
                    IsDiy = false,
                    Type = propertie.GetMethod.ReturnType,
                    IsPrimaryKey = false
                };

                list.Add(parameter);
            }

            return list;
        }
    }
}
