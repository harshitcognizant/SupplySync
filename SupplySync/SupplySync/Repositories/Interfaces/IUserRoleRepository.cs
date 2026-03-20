// /SupplySync/Repositories/Interfaces/IUserRoleRepository.cs
using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
	public interface IUserRoleRepository
	{

		Task<bool> ExistsAsync(int userId, int roleId);
		Task<UserRole?> GetActiveByUserAndRoleTypeAsync(int userId, RoleType roleType);
		Task<List<UserRole>> ListActiveByUserAsync(int userId);
		Task<UserRole> InsertAsync(UserRole userRole);
		Task<UserRole> UpdateAsync(UserRole userRole);
		Task<UserRole?> GetWithRoleAsync(int userRoleId);
		Task<UserRole?> GetAnyByUserAndRoleIdAsync(int userId, int roleId);

	}
}