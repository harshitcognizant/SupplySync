using SupplySync.DTOs.Report;
using SupplySync.Models;
using AutoMapper;

namespace SupplySync.Mappers
{

    public partial class MapperProfile
    {
        public void ConfigureReportMappings()
        {
            CreateMap<CreateReportRequestDto, Report>()
                .ForMember(dest => dest.GeneratedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateReportRequestDto, Report>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Report, ReportResponseDto>();

            CreateMap<Report, ReportListResponseDto>();
        }
    }

}
