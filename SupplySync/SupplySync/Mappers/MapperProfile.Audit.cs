using SupplySync.Constants.Enums;
using SupplySync.DTOs.Audit;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        public void ConfigureAuditMappings()
        {

            CreateMap<CreateAuditRequestDto, Audit>()
                            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConvertAuditStatus(src.Status)))
                            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateAuditRequestDto, Audit>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConvertAuditStatus(src.Status)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Audit, AuditResponseDto>();

            CreateMap<Audit, AuditListResponseDto>();
        }

        private static AuditStatus ConvertAuditStatus(string status)
        {
            return Enum.TryParse<AuditStatus>(status, true, out var parsed)
                ? parsed
                : AuditStatus.Planned;
        }

    }
}