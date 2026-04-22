using SupplySync.DTOs.Report;
using SupplySync.Models;
using AutoMapper;

namespace SupplySync.Mappers
{

    public partial class MapperProfile
    {
        public void ConfigureReportMappings()
        {
            CreateMap<CreateReportRequestDto, Report>();
            CreateMap<UpdateReportRequestDto, Report>();
            CreateMap<Report, ReportResponseDto>();
            CreateMap<Report, ReportListResponseDto>();
        }
    }

}
