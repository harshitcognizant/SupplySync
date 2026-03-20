// /SupplySync/Services/Interfaces/IUserRoleService.cs
using SupplySync.Constants.Enums;
using SupplySync.DTOs.UserRoles;

namespace SupplySync.Services.Interfaces
{
	public interface IUserRoleService
	{
		Task<UserRoleResponseDto> AssignRoleToUserAsync(int userId, CreateUserRoleRequestDto dto);
		Task<List<UserRoleResponseDto>> GetRolesForUserAsync(int userId);
		Task<UserRoleResponseDto> UpdateUserRoleAsync(int userId, UpdateUserRoleRequestDto dto);
		Task RemoveRoleFromUserAsync(int userId, RoleType roleType);
	}
}