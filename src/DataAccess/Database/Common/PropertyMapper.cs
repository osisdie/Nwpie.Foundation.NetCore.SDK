using System;
using System.Linq.Expressions;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal static class PropertyMapper
    {
        public static string[] GetPropertyMapNamesFromExps<TTable>(Expression<Func<TTable, object>>[] exps)
            where TTable : class
        {
            if (null == exps || 0 == exps.Length)
            {
                return new string[0];
            }

            var array = new string[exps.Length];
            for (var i = 0; i < exps.Length; i++)
            {
                var exp = exps[i];
                array[i] = GetPropertyMapNameFromExp<TTable, object>(exp);
            }

            return array;
        }

        public static string GetPropertyMapNameFromExp<TTable, TValue>(Expression<Func<TTable, TValue>> exp)
            where TTable : class
        {
            var parameterExpression = exp.Parameters[0];
            if (null == parameterExpression)
            {
                throw new ArgumentException($"No Parameter Expression In Exp:{exp} ");
            }

            string result;
            if (!(exp.Body is MemberExpression memberExpression))
            {
                if (!(exp.Body is UnaryExpression unaryExpression))
                {
                    throw new ArgumentException("value is invalud PropertyExpression. ");
                }

                var text = unaryExpression.ToString();
                var num = text.IndexOf(".", StringComparison.CurrentCulture) + 1;
                result = text.Substring(num, text.Length - num - 1).Replace('.', '_');
            }
            else
            {
                result = memberExpression.ToString().Substring(parameterExpression.Name.Length + 1).Replace('.', '_');
            }

            return result;
        }
    }
}
