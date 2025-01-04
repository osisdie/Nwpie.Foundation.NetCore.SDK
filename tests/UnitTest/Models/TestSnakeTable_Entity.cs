using System;
using Nwpie.Foundation.Abstractions.DataAccess.Models;

namespace Nwpie.xUnit.Models
{
    public class TestSnakeTable_Entity : EntityBase
    {
        public TestSnakeTable_Entity() : base("TestSnakeTable") { }

        public int id { get; set; }
        public string column_char { get; set; }
        public int? column_int { get; set; }
        public decimal? column_decimal { get; set; }
        public bool? column_bool { get; set; }
        public DateTime? column_date { get; set; }
        public DateTime? column_datetime { get; set; }
        public string column_not_exists { get; set; }
    }

    // AutoMapper
    public class TestSnakeTable_Converter : AutoMapper.Profile
    {
        public TestSnakeTable_Converter()
        {
            base.CreateMap<TestSnakeTable_Entity, TestTable_Entity>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(source => source.id))
                .ForMember(dest => dest.ColumnChar,
                    opts => opts.MapFrom(source => source.column_char))
                .ForMember(dest => dest.ColumnInt,
                    opts => opts.MapFrom(source => source.column_int))
                .ForMember(dest => dest.ColumnDecimal,
                    opts => opts.MapFrom(source => source.column_decimal))
                .ForMember(dest => dest.ColumnBool,
                    opts => opts.MapFrom(source => source.column_bool))
                .ForMember(dest => dest.ColumnDate,
                    opts => opts.MapFrom(source => source.column_date))
                .ForMember(dest => dest.ColumnDatetime,
                    opts => opts.MapFrom(source => source.column_datetime));
        }
    }
}
