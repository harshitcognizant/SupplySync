// /SupplySync/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Constants.Enums;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _context;
		public UserRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<User> InsertAsync(User user)
		{
			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
			return user;
		}

		public async Task<User?> GetByIdAsync(int id)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
		}

		public async Task<User> UpdateAsync(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return user;
		}

		public async Task<bool> EmailExistsAsync(string normalizedEmail)
		{
			return await _context.Users
				.AsNoTracking()
				.AnyAsync(u => u.Email == normalizedEmail && !u.IsDeleted);
		}

		public async Task<bool> EmailExistsForOtherAsync(string normalizedEmail, int excludeUserId)
		{
			return await _context.Users
				.AsNoTracking()
				.AnyAsync(u => u.Email == normalizedEmail && u.UserID != excludeUserId && !u.IsDeleted);
		}


		public async Task<User?> GetByIdWithRolesAsync(int id)
		{
			{
				return await _context.Users
				.AsNoTracking()
					.Include(u => u.UserRoles.Where(ur => !ur.IsDeleted))
						.ThenInclude(ur => ur.Role)
							.FirstOrDefaultAsync(u => u.UserID == id);
			}
		}

		public async Task<(List<User> Items, int TotalCount)> GetPagedWithRolesAsync(int pageNumber, int pageSize)
		{
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.Users
				.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
				.OrderBy(u => u.UserID); // deterministic ordering

			var total = await query.CountAsync();
			var items = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, total);
		}


		public async Task<(List<User> Items, int TotalCount)> GetUsersByRoleAsync(RoleType roleType, int pageNumber, int pageSize)
		{
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.Users
				.Where(u => !u.IsDeleted)
				.Include(u => u.UserRoles)
					.ThenInclude(ur => ur.Role)
				.Where(u => u.UserRoles.Any(ur => ur.Role.RoleType == roleType && !ur.IsDeleted && !ur.Role.IsDeleted))
				.OrderBy(u => u.UserID);

			var total = await query.CountAsync();
			var items = await query.Skip((pageNumber - 1) * pageSize)
								   .Take(pageSize)
								   .ToListAsync();

			return (items, total);
		}

		public async Task<User?> GetByEmailWithRolesAsync(string normalizedEmail)
		{
			return await _context.Users
				.Include(u => u.UserRoles)
					.ThenInclude(ur => ur.Role)
				.FirstOrDefaultAsync(u => u.Email == normalizedEmail && !u.IsDeleted);
		}

		public async Task<List<int>> GetActiveUserIdsByRoleTypesAsync(IEnumerable<RoleType> roleTypes)
		{
			var set = roleTypes?.Distinct().ToList() ?? new List<RoleType>();
			if (set.Count == 0) return new List<int>();

			return await _context.Users
				.Where(u => !u.IsDeleted)
				.Where(u => u.UserRoles.Any(ur => !ur.IsDeleted && !ur.Role.IsDeleted && set.Contains(ur.Role.RoleType)))
				.Select(u => u.UserID)
				.Distinct()
				.ToListAsync();
		}

	}
}