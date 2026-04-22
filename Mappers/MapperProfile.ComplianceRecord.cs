
using AutoMapper;
using SupplySync.DTOs.Compliance;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigureComplianceMappings()
        {
            // Create/Update DTO -> Model
            CreateMap<CreateComplianceRecordRequestDto, ComplianceRecord>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateComplianceRecordRequestDto, ComplianceRecord>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Model -> Response DTOs
            CreateMap<ComplianceRecord, ComplianceRecordResponseDto>();
            CreateMap<ComplianceRecord, ComplianceRecordListResponseDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result.ToString()));
        }
    }
}