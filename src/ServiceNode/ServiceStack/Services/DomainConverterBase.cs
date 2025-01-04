using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Mappers.Interfaces;
using Nwpie.Foundation.Common.Extras;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Services
{
    public abstract class DomainConverterBase : AutoMapper.Profile, IDomainConverter
    {
        public DomainConverterBase()
        {
            Init();
        }

        void Init()
        {
            Initialization();
        }

        public static T_ToDto ConvertTo<T_FromDto, T_ToDto>(T_FromDto src)
        {
            if (null == src)
            {
                return default(T_ToDto);
            }

            if (ComponentMgr.Instance.TryResolve<IMapperMgr>(out var mapper))
            {
                return mapper.ConvertTo<T_ToDto>(src);
            }

            return default(T_ToDto);
        }

        public static IEnumerable<T_ToDto> ConvertAll<T_FromDto, T_ToDto>(IEnumerable<T_FromDto> src)
        {
            if (null == src)
            {
                return null;
            }

            if (ComponentMgr.Instance.TryResolve<IMapperMgr>(out var mapper))
            {
                return mapper.ConvertAll<T_FromDto, T_ToDto>(src);
            }

            return Enumerable.Empty<T_ToDto>();
        }

        protected virtual void Initialization() { }
    }
}
