using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<User> InsertAsync(User user);
		Task<User?> GetByIdAsync(int id);
		Task<User?> GetByIdWithRolesAsync(int id);
		Task<User> UpdateAsync(User user);
		Task<bool> EmailExistsAsync(string normalizedEmail);
		Task<bool> EmailExistsForOtherAsync(string normalizedEmail, int excludeUserId);
		Task<(List<User> Items, int TotalCount)> GetPagedWithRolesAsync(int pageNumber, int pageSize);
		Task<(List<User> Items, int TotalCount)> GetUsersByRoleAsync(RoleType roleType, int pageNumber, int pageSize);
		Task<User?> GetByEmailWithRolesAsync(string normalizedEmail);
		Task<List<int>> GetActiveUserIdsByRoleTypesAsync(IEnumerable<RoleType> roleTypes);
	}
}
