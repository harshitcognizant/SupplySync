// /SupplySync/Repositories/UserRoleRepository.cs
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Constants.Enums;

namespace SupplySync.Repositories
{
	public class UserRoleRepository : IUserRoleRepository
	{
		private readonly AppDbContext _context;
		public UserRoleRepository(AppDbContext context) => _context = context;

		public async Task<bool> ExistsAsync(int userId, int roleId)
		{
			return await _context.UserRoles
				.AsNoTracking()
				.AnyAsync(ur => ur.UserID == userId && ur.RoleID == roleId && !ur.IsDeleted);
		}

		public async Task<UserRole?> GetActiveByUserAndRoleTypeAsync(int userId, RoleType roleType)
		{
			return await _context.UserRoles
				.Include(ur => ur.Role)
				.FirstOrDefaultAsync(ur =>
					ur.UserID == userId &&
					!ur.IsDeleted &&
					ur.Role.RoleType == roleType &&
					!ur.Role.IsDeleted);
		}

		public async Task<List<UserRole>> ListActiveByUserAsync(int userId)
		{
			return await _context.UserRoles
				.Include(ur => ur.Role)
				.Where(ur => ur.UserID == userId && !ur.IsDeleted && !ur.Role.IsDeleted)
				.OrderBy(ur => ur.RoleID)
				.ToListAsync();
		}

		public async Task<UserRole> InsertAsync(UserRole userRole)
		{
			await _context.UserRoles.AddAsync(userRole);
			await _context.SaveChangesAsync();
			return userRole;
		}

		public async Task<UserRole> UpdateAsync(UserRole userRole)
		{
			_context.UserRoles.Update(userRole);
			await _context.SaveChangesAsync();
			return userRole;
		}

		public async Task<UserRole?> GetWithRoleAsync(int userRoleId)
		{
			return await _context.UserRoles
				.Include(ur => ur.Role)
				.FirstOrDefaultAsync(ur => ur.UserRoleID == userRoleId);
		}

		public async Task<UserRole?> GetAnyByUserAndRoleIdAsync(int userId, int roleId)
		{
			return await _context.UserRoles
				.FirstOrDefaultAsync(ur => ur.UserID == userId && ur.RoleID == roleId);
		}

	}
}