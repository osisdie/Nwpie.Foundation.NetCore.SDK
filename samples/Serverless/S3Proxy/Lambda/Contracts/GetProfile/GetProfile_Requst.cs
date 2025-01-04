namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts.GetProfile
{
    public class GetProfile_Request
    {
        public GetProfile_RequestModel Data { get; set; } = new GetProfile_RequestModel();
    }

    public class GetProfile_RequestModel
    {
        public string AccountId { get; set; }
        public string Email { get; set; }
    }
}
