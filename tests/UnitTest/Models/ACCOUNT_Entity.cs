using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nwpie.Foundation.Abstractions.DataAccess.Models;

namespace Nwpie.xUnit.Models
{
    [Table("ACCOUNT_VIEW")]
    public class ACCOUNT_VIEW_Entity : EntityBase
    {
        public ACCOUNT_VIEW_Entity() : base("ACCOUNT_VIEW") { }

        public string parent_account_id { get; set; }

        #region account
        public string account_id { get; set; }
        //public string status { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }
        #endregion

        #region detail
        public string country_code { get; set; }
        public byte? gender { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string company_email { get; set; }
        public string backup_email { get; set; }
        public string json_tag { get; set; }
        public string json_metadata { get; set; }
        public string json_profile { get; set; }
        public string json_perference { get; set; }
        public bool? is_parent { get; set; }
        #endregion

        #region credential
        [Key]
        public string credential_key { get; set; }
        public byte? credential_kind { get; set; }
        public string account_secret { get; set; }
        public string salt { get; set; }
        public string env { get; set; }
        public DateTime? expire_date { get; set; }
        public DateTime? last_access_date { get; set; }
        public string last_access_account_id { get; set; }
        public bool? is_credential_deleted { get; set; }
        public string credential_json_metadata { get; set; }
        #endregion
    }
}
