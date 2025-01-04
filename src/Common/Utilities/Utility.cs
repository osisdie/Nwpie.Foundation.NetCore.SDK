using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging.Attributes;
using Nwpie.Foundation.Abstractions.Statics;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Common.Utilities
{
    public static class Utility
    {
        public static string ErrMsgDependOnEnv(Exception ex)
        {
            return ServiceContext.IsDebugOrDevelopment()
                ? ex.GetBaseFirstExceptionString()
                : ex.GetBaseFirstExceptionMessage();
        }

        public static string GetSDKAppNameByApiName(string apiName)
        {
            if (true == apiName?.Contains(ConfigConst.ApiNameDivider))
            {
                var env = GetSDKEnvNameByApiName(apiName);
                if (env.HasValue())
                {
                    return apiName.Remove(apiName.Length - (env.Length + ConfigConst.ApiNameDivider.ToString().Length));
                }
            }

            return apiName;
        }

        public static string GetSDKEnvNameByApiName(string apiName)
        {
            if (true == apiName?.Contains(ConfigConst.ApiNameDivider))
            {
                var env = apiName.Split(ConfigConst.ApiNameDivider).Last();
                if (EnvironmentEnum.Max != Enum<EnvironmentEnum>.TryParseFromDisplayAttr(env, EnvironmentEnum.Max))
                {
                    return env;
                }
            }

            if (EnvironmentEnum.Max != Enum<EnvironmentEnum>.TryParseFromDisplayAttr(apiName, EnvironmentEnum.Max))
            {
                return apiName;
            }

            return string.Empty;
        }

        public static void CopyToLogDto(object parent, IDictionary<string, object> child)
        {
            var parentProperties = parent.GetType().GetProperties();
            foreach (var p in parentProperties)
            {
                if (p.IsDefined(typeof(IgnoreLoggingAttribute), true))
                {
                    var val = p.GetType().IsValueType
                        ? Activator.CreateInstance(p.GetType())
                        : null;
                    AddLogProperty(child, p.Name, val);
                    continue;
                }

                if (p.IsDefined(typeof(IncludeLoggingAttribute), true))
                {
                    AddLogProperty(child, p.Name, p.GetValue(parent));
                    continue;
                }

                if (p.PropertyType.IsGenericType &&
                    typeof(List<>) == p.PropertyType.GetGenericTypeDefinition())
                {
                    var items = p.GetValue(parent);
                    var val = (items as IEnumerable<object>)
                        ?.Count()
                        ?? 0;
                    AddLogProperty(child, $"{p.Name}Count", val);
                    continue;
                }

                if (string.Equals(p.Name, nameof(IServiceResponse<object>.Data), StringComparison.OrdinalIgnoreCase) &&
                    null != p.GetValue(parent))
                {
                    AddLogProperty(child, p.Name, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
                    CopyToLogDto(p.GetValue(parent), GetLogProperty(child, p.Name) as IDictionary<string, object>);
                    continue;
                }

                AddLogProperty(child, p.Name, p.GetValue(parent));
            }
        }

        public static void AddLogProperty(IDictionary<string, object> dict, string propName, object propValue)
        {
            if (false == dict.ContainsKey(propName))
            {
                dict.Add(propName, propValue);
                return;
            }

            dict[propName] = propValue;
        }

        public static object GetLogProperty(IDictionary<string, object> dict, string propName)
        {
            return dict?[propName];
        }

        public static string IntToLetters(int value)
        {
            var result = string.Empty;
            while (--value >= 0)
            {
                result = (char)('A' + value % 26) + result;
                value /= 26;
            }

            return result;
        }

        // https://gist.github.com/rickdailey/8679306
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            var table = new DataTable();
            var properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static string ToHtmlTable<T>(this List<T> items)
        {
            var ret = string.Empty;
            return null == items ||
                false == items.Any()
                ? ret
                : ("<table cellspacing='0' cellpadding='1' border='1'>" +
                    items.First()
                        .GetType()
                        .GetProperties()
                        .Where(o => false == IsJsonIgnored(o))
                        .Select(p => string.IsNullOrWhiteSpace(
                            GetPropertyValue(typeof(DisplayAttribute), "Description", p))
                            ? p.Name
                            : GetPropertyValue(typeof(DisplayAttribute), "Description", p)
                        ).ToList()
                        .ToColumnHeaders() +
                    items.Aggregate(ret, (current, t) =>
                        current + t.ToHtmlTableRow()
                    ) + "</table>"
                );
        }

        public static bool IsJsonIgnored(PropertyInfo prop)
        {
            return true == prop.GetCustomAttributes(true)
                ?.Where(o => o.GetType() == typeof(JsonIgnoreAttribute))
                ?.Any();
        }

        public static string GetPropertyValue(Type type, string argName, PropertyInfo prop)
        {
            string value = null;
            var d = (
                from q in prop.GetCustomAttributesData()
                where q.ToString().Contains(type.FullName)
                select q
            ).FirstOrDefault();

            if (null != d)
            {
                var d2 = (
                    from q in d.NamedArguments
                    where q.MemberInfo.Name == argName
                    select q.TypedValue.Value
                ).FirstOrDefault();

                if (null != d2)
                {
                    value = d2.ToString();
                }
            }

            return value;
        }

        public static string ToColumnHeaders<T>(this List<T> items)
        {
            var ret = string.Empty;

            return null == items ||
                false == items.Any()
                ? ret
                : ("<tr>" +
                    items.Aggregate(ret,
                        (current, propValue) =>
                            string.Concat(current,
                                "<th style='font-size: 11pt; font-weight: bold; background-color:blue; color:white'>",
                                (Convert.ToString(propValue).Length <= 100
                                    ? Convert.ToString(propValue)
                                    : Convert.ToString(propValue)
                                ).Substring(0, 100),
                                "</th>"
                            )
                    ) + "</tr>"
                );
        }

        public static string ToHtmlTableRow<T>(this T data)
        {
            var ret = string.Empty;

            return null == data
                ? ret
                : ("<tr style='text-align:left'>" +
                    data.GetType()
                        .GetProperties()
                        .Where(o => false == IsJsonIgnored(o))
                        .Aggregate(ret,
                            (current, prop) =>
                                string.Concat(current,
                                    "<td style='font-size: 11pt; font-weight: normal;'>",
                                    (Convert.ToString(prop.GetValue(data, null)).Length <= 100
                                        ? Convert.ToString(prop.GetValue(data, null))
                                        : Convert.ToString(prop.GetValue(data, null)
                                    ).Substring(0, 100)),
                                    "</td>"
                                )
                        ) + "</tr>"
                    );
        }

        public static bool IsValidXhtml(this string value)
        {
            var isOK = false;
            try
            {
                if (value.HasValue())
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(value);
                    isOK = true;
                }
            }
            catch { }

            return isOK;
        }

        public static string FormatExcelUrl(string url, string display) =>
            string.Format("{0},{1}", url, display);

        public static bool IsFileReadyToMove(string fileName)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                var errorCode = Marshal.GetHRForException(ex) & (1 << 16) - 1;
                if (ex is IOException &&
                    (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                {
                    return false;
                }

                return false;
            }
            finally
            {
                stream?.Close();
            }

            return true;
        }

        #region Reflection
        public static string GetCallerFullName(int nth = 1)
        {
            try
            {
                var deepMethod = new StackTrace().GetFrame(nth).GetMethod();
                return $"{deepMethod.ReflectedType.Name}.{deepMethod.Name}";
            }
            catch { }

            var firstMethod = new StackTrace().GetFrame(1).GetMethod();
            return $"{firstMethod.ReflectedType.Name}.{firstMethod.Name}";
        }

        public static string GetCallerAsmName()
        {
            return Assembly
                .GetCallingAssembly()
                .GetName()
                .Name;
        }

        public static string GetCallerAsmVersion()
        {
            return Assembly
                .GetCallingAssembly()
                .GetName()
                .Version
                .ToString();
        }
        #endregion

        #region Crypto
        public static string ToMD5(this string src, Encoding encoding = null) =>
            CryptoUtils.GetMD5String(src, encoding);

        public static string ToSha1(this string src, Encoding encoding = null) =>
            CryptoUtils.GetSha1String(src, encoding);

        public static string ToSha256(this string src, Encoding encoding = null) =>
            CryptoUtils.GetSha256String(src, encoding);
        #endregion

        public const int ERROR_SHARING_VIOLATION = 32;
        public const int ERROR_LOCK_VIOLATION = 33;

        public static readonly Random Randomizer = new Random();
    }
}
