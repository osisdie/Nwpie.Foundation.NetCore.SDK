using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.Storage.Contract.Test;

namespace Nwpie.MiniSite.Storage.ServiceCore.Test
{
    public class Test_DomainConverter : DomainConverterBase
    {
        public Test_DomainConverter()
        {
            CreateMap<Test_Request, Test_Response>()
                .ForMember(dest => dest.EchoBack,
                    opts => opts.MapFrom(source => source.Echo));
        }

        public static Test_Response ConvertToResponseModel(Test_Request from)
        {
            return ConvertTo<Test_Request, Test_Response>(from);
        }
    }
}
