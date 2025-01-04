using Nwpie.Foundation.ServiceNode.HealthCheck.Contracts;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Models
{
    public class HlckEcho_Converter : DomainConverterBase
    {
        public HlckEcho_Converter()
        {
            base.CreateMap<HlckEcho_Entity, HlckEcho_ResponseModel>()
                .ForMember(dest => dest.ResponseString,
                    opts => opts.MapFrom(source => source.words));
        }

        public static HlckEcho_ResponseModel ConvertToResponseModel(HlckEcho_Entity from) =>
            ConvertTo<HlckEcho_Entity, HlckEcho_ResponseModel>(from);
    }
}
