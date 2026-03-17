using SupplySync.DTOs.Compliance;
using SupplySync.Models;

namespace SupplySync.Mappers
{

    public partial class MapperProfile

    {
        public void ConfigureComplianceRecordMappings()
        {
            CreateMap<CreateComplianceRecordRequestDto, ComplianceRecord>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateComplianceRecordRequestDto, ComplianceRecord>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ComplianceRecord, ComplianceRecordResponseDto>();

            CreateMap<ComplianceRecord, ComplianceRecordListResponseDto>();
        }

    }
}
