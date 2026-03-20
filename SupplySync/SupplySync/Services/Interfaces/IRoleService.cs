// /SupplySync/Services/Interfaces/IRoleService.cs
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Role;
using SupplySync.DTOs.User;

namespace SupplySync.Services.Interfaces
{
	public interface IRoleService
	{
		Task<RoleListResponseDto> ListRolesAsync(int pageNumber, int pageSize);

		Task<UserListResponseDto> ListUsersByRoleAsync(RoleType roleType, int pageNumber, int pageSize);
	}
}
