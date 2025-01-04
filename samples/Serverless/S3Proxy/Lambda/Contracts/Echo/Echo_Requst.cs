namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts.Echo
{
    public class Echo_Request
    {
        public Echo_RequestModel Data { get; set; } = new Echo_RequestModel();
    }

    public class Echo_RequestModel
    {
        public string Words { get; set; }
    }
}
