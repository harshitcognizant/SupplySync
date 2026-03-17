using SupplySync.Constants.Enums;
using SupplySync.DTOs.Audit;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigureAuditMappings()
        {
            CreateMap<CreateAuditDto, Audit>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConvertStatus(src.Status)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));
        }

        private static AuditStatus ConvertStatus(string status)
        {
            return Enum.TryParse<AuditStatus>(status, true, out var parsed)
                ? parsed
                : AuditStatus.Planned;
        }
    }
}