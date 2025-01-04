namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts.GetProfile
{
    public class GetProfile_Pesponse : ResponseDtoBase
    {
        public GetProfile_PesponseModel Data { get; set; }
    }

    public class GetProfile_PesponseModel
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
        public string Email { get; set; }
    }
}
