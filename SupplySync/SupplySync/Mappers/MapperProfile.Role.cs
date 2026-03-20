// /SupplySync/Mappers/Mapper.Role.cs
using AutoMapper;
using SupplySync.DTOs.Role;
using SupplySync.Models;

namespace SupplySync.Mappers
{
	public partial class MapperProfile
	{
		private void ConfigureRoleMappings()
		{
			CreateMap<Role, RoleResponseDto>();
		}
	}
}