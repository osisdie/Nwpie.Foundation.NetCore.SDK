using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Enums;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class EnvironmentExtension
    {
        public static IEnumerable<EnvironmentEnum> TakeNonProduction(this IEnumerable<EnvironmentEnum> src)
        {
            foreach (var item in src)
            {
                if ((int)item <= (int)EnvironmentEnum.Staging)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<EnvironmentEnum> TakeProduction(this IEnumerable<EnvironmentEnum> src)
        {
            foreach (var item in src)
            {
                if ((int)item >= (int)EnvironmentEnum.Staging_2)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<string> TakeNonProduction(this IEnumerable<string> src)
        {
            foreach (var item in src)
            {
                if ((int)Enum<EnvironmentEnum>.ParseFromDisplayAttr(item) <= (int)EnvironmentEnum.Staging)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<string> TakeProduction(this IEnumerable<string> src)
        {
            foreach (var item in src)
            {
                if ((int)Enum<EnvironmentEnum>.ParseFromDisplayAttr(item) >= (int)EnvironmentEnum.Staging_2)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> TakeNonProduction(this IEnumerable<KeyValuePair<string, string>> src)
        {
            foreach (var item in src)
            {
                if ((int)Enum<EnvironmentEnum>.ParseFromDisplayAttr(item.Key) <= (int)EnvironmentEnum.Staging)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> TakeProduction(this IEnumerable<KeyValuePair<string, string>> src)
        {
            foreach (var item in src)
            {
                if ((int)Enum<EnvironmentEnum>.ParseFromDisplayAttr(item.Key) >= (int)EnvironmentEnum.Staging_2)
                {
                    yield return item;
                }
            }
        }
    }
}
