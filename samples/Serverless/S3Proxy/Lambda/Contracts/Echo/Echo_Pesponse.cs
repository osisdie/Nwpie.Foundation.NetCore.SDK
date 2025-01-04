namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts.Echo
{
    public class Echo_Pesponse : ResponseDtoBase
    {
        public Echo_PesponseModel Data { get; set; }
    }

    public class Echo_PesponseModel
    {
        public string Words { get; set; }
    }
}
