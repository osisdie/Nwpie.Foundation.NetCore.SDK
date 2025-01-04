using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Enums
{
    public enum CloudProviderEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "aws")]
        AWS = 1, //Amazon web service

        [Display(Name = "azure")]
        Azure = 2, //Microsoft Azure

        [Display(Name = "gcp")]
        GCP = 3, //Google Cloud Platform

    }
}
