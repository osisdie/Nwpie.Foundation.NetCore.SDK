using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.MiniSite.KVS.Common.Domain;
using Nwpie.MiniSite.KVS.Common.Utilities;
using Nwpie.MiniSite.KVS.Contract.Entities;

namespace Nwpie.MiniSite.KVS.Common.Entities
{
    [Table("API_KEY")]
    public class API_KEY_Entity : KvsEntity
    {
        public API_KEY_Entity() : base("API_KEY") { }

        public string parent_apikey { get; set; }

        [Key]
        public string apikey { get; set; }
        public string app_id { get; set; }
        public string sys_name { get; set; }
        public string env { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string secretkey { get; set; }
        public int? access_level { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }

    [Table("CONFIG_KEY")]
    public class CONFIG_KEY_Entity : KvsEntity
    {
        public CONFIG_KEY_Entity() : base("CONFIG_KEY") { }

        [Key]
        public string configkey { get; set; }
        [Key]
        public string app_id { get; set; }
        public string section { get; set; }
        public DateTime? latest_value_updated { get; set; }
        public int? access_level { get; set; }
        public int? refresh_secs { get; set; }
        public bool? encrypted { get; set; }
        public bool? privileged { get; set; }
    }

    [Table("CONFIG_VALUES")]
    public class CONFIG_VALUES_Entity : KvsEntity
    {
        public CONFIG_VALUES_Entity() : base("CONFIG_VALUES") { }

        [Key]
        public string configkey { get; set; }
        [Key]
        public string app_id { get; set; }
        [Key]
        public string apikey { get; set; }
        [Key]
        public string version { get; set; }
        public string version_display { get; set; }
        public string rawdata { get; set; }
        public bool? encrypted { get; set; }

        [IgnoreDataMember]
        public int? distance { get; set; }
    }

    [Table("PERMISSION_CONFIG_KEY")]
    public class PERMISSION_CONFIG_KEY_Entity : KvsEntity
    {
        public PERMISSION_CONFIG_KEY_Entity() : base("PERMISSION_CONFIG_KEY") { }

        [Key]
        public string configkey { get; set; }
        [Key]
        public string app_id { get; set; }
        [Key]
        public string target_id { get; set; }
        [Key]
        public string target_type { get; set; }
        public string perm_id { get; set; }
        public int? weight { get; set; }
        public bool? blocked { get; set; }
    }

    public static class CONFIG_VALUES_Entity_Extension
    {
        public static string DecryptData(this CONFIG_VALUES_Entity o) =>
            true == o.encrypted
            ? CryptoUtils.DecodeFromAES(o.rawdata, KvsConfigUtils.AuthOption.AuthAESKey, KvsConfigUtils.AuthOption.AuthAESIV)
            : o.rawdata;
    }

    public static class PERMISSION_CONFIG_KEY_Entity_Extension
    {
        public static bool IsAllowAccess(this IEnumerable<PERMISSION_CONFIG_KEY_Entity> permissionList, List<CONFIG_VALUES_Entity> valueList, string ownerAppId, string accessAppId)
        {
            var isBlock = permissionList
                ?.Any(x => true == x.blocked &&
                    x.app_id == ownerAppId &&
                    x.target_id == accessAppId &&
                    x.target_type == Enum<PermissionTypeEnum>.GetDispValue(PermissionTypeEnum.AllowAppId)
                );

            if (true == isBlock)
            {
                return false;
            }

            var isAllow = permissionList?.Any(x =>
                false == x.blocked &&
                x.app_id == ownerAppId &&
                x.target_id == accessAppId &&
                x.target_type == Enum<PermissionTypeEnum>.GetDispValue(PermissionTypeEnum.AllowAppId)
            );

            if (true == isAllow)
            {
                return true;
            }

            var isSelf =
                ownerAppId == accessAppId &&
                true == valueList?.Any(x => x.app_id == ownerAppId);
            if (isSelf)
            {
                return true;
            }

            return false;
        }

        public static async Task<PERMISSION_CONFIG_KEY_Entity> SetPermissionWeight(this PERMISSION_CONFIG_KEY_Entity o, PermissionLevelEnum level)
        {
            var perm = await PermissionDomainHelper.GetPermissionByNameAsync(Enum<PermissionLevelEnum>.GetDispValue(level));
            if (null != perm)
            {
                o.perm_id = perm.perm_id;
                o.weight = perm.weight;
            }
            else
            {
                o.perm_id = "";
                o.weight = 0;
            }

            return o;
        }
    }
}
