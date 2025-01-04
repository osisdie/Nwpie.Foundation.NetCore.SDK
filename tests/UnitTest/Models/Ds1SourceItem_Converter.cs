using Nwpie.MiniSite.ES.Contract.Entities;
using Nwpie.MiniSite.ES.Contract.Models;
using Newtonsoft.Json;

namespace Nwpie.xUnit.Models
{
    // AutoMapper
    public class Ds1SourceItem_Converter : AutoMapper.Profile
    {
        public Ds1SourceItem_Converter()
        {
            base.CreateMap<Ds1SourceItem_Entity, Ds1SourceItem>()
                .ForMember(dest => dest.itemGuid,
                    opts => opts.MapFrom(source => source.item_guid))
                .ForMember(dest => dest.nsRecordType,
                    opts => opts.MapFrom(source => source.ns_record_type))
                .ForMember(dest => dest.version,
                    opts => opts.MapFrom(source => source.version))
                .ForMember(dest => dest.status,
                    opts => opts.MapFrom(source => source.status))
                .ForMember(dest => dest.isDeleted,
                    opts => opts.MapFrom(source => source.is_deleted))
                .ForMember(dest => dest.isHidden,
                    opts => opts.MapFrom(source => source.is_hidden))
                .ForMember(dest => dest.nsItemId,
                    opts => opts.MapFrom(source => source.ns_item_id))
                .ForMember(dest => dest.nsInternalId,
                    opts => opts.MapFrom(source => source.ns_internal_id))
                .ForMember(dest => dest.color,
                    opts => opts.MapFrom(source => source.color))
                .ForMember(dest => dest.modifyAt,
                    opts => opts.MapFrom(source => source.modify_at))
                .ForMember(dest => dest.createAt,
                    opts => opts.MapFrom(source => source.create_at))
                .ForMember(dest => dest.matWood,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).MatWood : string.Empty))
                .ForMember(dest => dest.upcCode,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).UpcCode : string.Empty))
                .ForMember(dest => dest.boxWidth,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).BoxWidth : string.Empty))
                .ForMember(dest => dest.category,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).Category : string.Empty))
                .ForMember(dest => dest.matMetal,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).MatMetal : string.Empty))
                .ForMember(dest => dest.boxHeight,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).BoxHeight : string.Empty))
                .ForMember(dest => dest.boxLength,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).BoxLength : string.Empty))
                .ForMember(dest => dest.cubicFeet,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).CubicFeet : string.Empty))
                .ForMember(dest => dest.matMirror,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).MatMirror : string.Empty))
                .ForMember(dest => dest.netWeight,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).NetWeight : string.Empty))
                .ForMember(dest => dest.matCushion,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).MatCushion : string.Empty))
                .ForMember(dest => dest.setupDepth,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).SetupDepth : string.Empty))
                .ForMember(dest => dest.setupWidth,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).SetupWidth : string.Empty))
                .ForMember(dest => dest.grossWeight,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).GrossWeight : string.Empty))
                .ForMember(dest => dest.setupHeight,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).SetupHeight : string.Empty))
                .ForMember(dest => dest.subCategory,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).SubCategory : string.Empty))
                .ForMember(dest => dest.setupDimension,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).SetupDimension : string.Empty))
                .ForMember(dest => dest.itemDescription,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).ItemDescription : string.Empty))
                .ForMember(dest => dest.matOthersFabric,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).MatOthersFabric : string.Empty))
                .ForMember(dest => dest.setupFootboardHeight,
                    opts => opts.MapFrom(source => null != source.ns_data ? JsonConvert.DeserializeObject<Ds1SourceItemJsonData>(source.ns_data).SetupFootboardHeight : string.Empty))
                ;
        }
    }
}
