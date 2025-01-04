using System.ComponentModel.DataAnnotations;

namespace Nwpie.MiniSite.KVS.Contract.Enums
{
    public enum KvsModuleKindEnum
    {
        [Display(Name = "ltnl")]
        Internal,

        [Display(Name = "configserver")]
        ConfigServer,

        [Display(Name = "hlck")]
        HealthCheck,
    }
}
