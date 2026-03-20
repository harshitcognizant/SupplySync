using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
	public interface IRoleRepository
	{


		Task<(List<Role> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
		Task<Role?> GetByTypeAsync(RoleType roleType);

	}
}
