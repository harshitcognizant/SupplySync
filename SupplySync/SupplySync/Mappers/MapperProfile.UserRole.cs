// /SupplySync/Mappers/Mapper.UserRole.cs
using AutoMapper;
using SupplySync.DTOs.UserRoles;
using SupplySync.Models;

namespace SupplySync.Mappers
{
	public partial class MapperProfile
	{
		private void ConfigureUserRoleMappings()
		{
			CreateMap<UserRole, UserRoleResponseDto>()
				.ForMember(d => d.RoleType, opt => opt.MapFrom(src => src.Role.RoleType.ToString()));
		}
	}
}