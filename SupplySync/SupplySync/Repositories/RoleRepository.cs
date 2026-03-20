// SupplySync/Repositories/RoleRepository.cs
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Constants.Enums;

namespace SupplySync.Repositories
{
	public class RoleRepository : IRoleRepository
	{
		private readonly AppDbContext _context;
		public RoleRepository(AppDbContext context) => _context = context;

		public async Task<(List<Role> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
		{
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.Roles
				.Where(r => !r.IsDeleted)
				.OrderBy(r => r.RoleID);

			var total = await query.CountAsync();
			var items = await query.Skip((pageNumber - 1) * pageSize)
								   .Take(pageSize)
								   .ToListAsync();

			return (items, total);
		}

		public async Task<Role?> GetByTypeAsync(RoleType roleType)
		{
			return await _context.Roles
				.FirstOrDefaultAsync(r => r.RoleType == roleType && !r.IsDeleted);
		}
	}
}