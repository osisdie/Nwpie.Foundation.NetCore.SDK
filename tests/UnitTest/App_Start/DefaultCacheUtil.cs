using System;
using System.Security.Cryptography;
using System.Text;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;

namespace Nwpie.xUnit.App_Start
{
    public static class DefaultCacheUtil
    {
        public static (string old, string @new) OverwriteRedisCacheConfig(string newConfig = null)
        {
            string oldConfig = ServiceContext.Configuration?[SysConfigKey.Default_AWS_Redis_ConnectionString_ConfigKey];
            if (string.IsNullOrEmpty(newConfig))
            {
                newConfig = GenRedisConnectionString();
            }
            ServiceContext.Configuration[SysConfigKey.Default_AWS_Redis_ConnectionString_ConfigKey] = newConfig;
            return (oldConfig, newConfig);
        }

        private static string GenRedisConnectionString()
        {
            var connStr = "localhost:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0,abortConnect=false";
            //var connStr = "127.0.0.1:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0,abortConnect=false";
            //var connStr = "172.18.140.253:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0";

            RedisCache_Option option = new()
            {
                ConnectionString = connStr
            };

            return option.ConnectionString;
        }
    }
}
