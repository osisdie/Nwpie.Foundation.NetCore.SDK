using System;

namespace Nwpie.xUnit.Models
{
    public class SvcAssets_AwsCfg
    {
        /// <summary>
        /// 連到 AWS 服務所需要的資訊，類似帳號
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// 連到 AWS 服務所需要的資訊，類似密碼
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 連到 AWS 服務所需要的資訊，選擇地區
        /// </summary>
        public string Region { get; set; }

        public DateTime? _ts { get; set; }
    }
}
