using SupplySync.DTOs.AuditLogs;
using SupplySync.Models;

namespace SupplySync.Mappers
{
	public partial class MapperProfile
	{
		private void ConfigureAuditLogMappings()
		{
			CreateMap<AuditLog, AuditLogResponseDto>()
				.ForMember(d => d.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null));
		}
	}

}
