using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.CreateToken
{
    public class TkCreateApiKey_RequestModel : TkCreateToken_RequestModel
    {
        public override string AccessKey { get; set; } // ApiKey, Bearer.ToMD5(), PAT.ToMD5()
        public override byte? Kind { get; set; }
        public override byte? Level { get; set; }
    }

    public class TkCreatePAT_RequestModel : TkCreateToken_RequestModel
    {

    }

    public class TkCreateBearer_RequestModel : TkCreateToken_RequestModel
    {

    }

    public class TkCreateToken_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(IsRequired = true)]
        public virtual string Name { get; set; } // Display name

        [Required]
        [ApiMember(IsRequired = true)]
        public virtual DateTime? ExpireDate { get; set; }

        [StringLength(ConfigConst.MaxTextLength)]
        public virtual string Description { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public override string AccountId { get; set; }

        [Obsolete]
        public virtual string Env { get; set; }

        #region Hidden fields
        [IgnoreDataMember]
        public virtual string AccessKey { get; set; } // ApiKey, Bearer.ToMD5(), PAT.ToMD5()

        [IgnoreDataMember]
        public virtual byte? Kind { get; set; }

        [IgnoreDataMember]
        public virtual byte? Level { get; set; }
        #endregion
    }
}
