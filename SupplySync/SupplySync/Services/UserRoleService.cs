// /SupplySync/Services/UserRoleService.cs
using AutoMapper;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.UserRoles;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SupplySync.Services
{
	public class UserRoleService : IUserRoleService
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoleRepository _roleRepository;
		private readonly IUserRoleRepository _userRoleRepository;
		private readonly IMapper _mapper;

		public UserRoleService(
			IUserRepository userRepository,
			IRoleRepository roleRepository,
			IUserRoleRepository userRoleRepository,
			IMapper mapper)
		{
			_userRepository = userRepository;
			_roleRepository = roleRepository;
			_userRoleRepository = userRoleRepository;
			_mapper = mapper;
		}

		public async Task<UserRoleResponseDto> AssignRoleToUserAsync(int userId, CreateUserRoleRequestDto dto)
		{
			// 1) Validate user
			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new KeyNotFoundException("User not found.");
			if (user.IsDeleted)
				throw new InvalidOperationException("Cannot assign roles to a deleted user.");

			// 2) Resolve role by RoleType (per your enum-seeded roles)
			var role = await _roleRepository.GetByTypeAsync(dto.RoleType)
				?? throw new KeyNotFoundException("Role not found.");
			if (role.IsDeleted)
				throw new InvalidOperationException("Cannot assign a deleted role.");

			// 3) If active link exists -> conflict
			if (await _userRoleRepository.ExistsAsync(userId, role.RoleID))
				throw new InvalidOperationException("Role already assigned to the user.");

			// 4) Try to find ANY link (even soft-deleted)
			var any = await _userRoleRepository.GetAnyByUserAndRoleIdAsync(userId, role.RoleID);
			if (any is not null)
			{
				// ✅ Reactivate soft-deleted link
				if (any.IsDeleted)
				{
					any.IsDeleted = false;
					any.UpdatedAt = DateTime.UtcNow;
					var reactivated = await _userRoleRepository.UpdateAsync(any);

					var loaded = await _userRoleRepository.GetWithRoleAsync(reactivated.UserRoleID)
						?? throw new KeyNotFoundException("Reactivated user role could not be retrieved.");

					return _mapper.Map<UserRoleResponseDto>(loaded);
				}

				// If not deleted and ExistsAsync returned false (shouldn't happen), treat as conflict
				throw new InvalidOperationException("Role already assigned to the user.");
			}

			// 5) No link at all -> insert new
			var userRole = new UserRole
			{
				UserID = userId,
				RoleID = role.RoleID,
				IsDeleted = false,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			var created = await _userRoleRepository.InsertAsync(userRole);
			var loadedNew = await _userRoleRepository.GetWithRoleAsync(created.UserRoleID)
				?? throw new KeyNotFoundException("Assigned role could not be retrieved.");

			return _mapper.Map<UserRoleResponseDto>(loadedNew);
		}



		public async Task<List<UserRoleResponseDto>> GetRolesForUserAsync(int userId)
		{
			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new KeyNotFoundException("User not found.");
			if (user.IsDeleted)
				return new List<UserRoleResponseDto>();

			var items = await _userRoleRepository.ListActiveByUserAsync(userId);
			return _mapper.Map<List<UserRoleResponseDto>>(items);
		}

		public async Task<UserRoleResponseDto> UpdateUserRoleAsync(int userId, UpdateUserRoleRequestDto dto)
		{
			if (dto.CurrentRoleType == dto.NewRoleType)
				throw new InvalidOperationException("Current and new role types are the same.");

			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new KeyNotFoundException("User not found.");
			if (user.IsDeleted)
				throw new InvalidOperationException("Cannot update roles for a deleted user.");

			var current = await _userRoleRepository.GetActiveByUserAndRoleTypeAsync(userId, dto.CurrentRoleType)
				?? throw new KeyNotFoundException("Current role is not assigned to the user.");

			var newRole = await _roleRepository.GetByTypeAsync(dto.NewRoleType)
				?? throw new KeyNotFoundException("New role not found.");
			if (newRole.IsDeleted)
				throw new InvalidOperationException("Cannot assign a deleted role.");

			// Prevent duplicate assignment
			if (await _userRoleRepository.ExistsAsync(userId, newRole.RoleID))
				throw new InvalidOperationException("User already has the target role.");

			// Update the same UserRole row to the new role
			current.RoleID = newRole.RoleID;
			current.UpdatedAt = DateTime.UtcNow;

			var updated = await _userRoleRepository.UpdateAsync(current);
			var loaded = await _userRoleRepository.GetWithRoleAsync(updated.UserRoleID)
				?? throw new KeyNotFoundException("Updated user role could not be retrieved.");

			return _mapper.Map<UserRoleResponseDto>(loaded);
		}

		public async Task RemoveRoleFromUserAsync(int userId, RoleType roleType)
		{
			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new KeyNotFoundException("User not found.");
			if (user.IsDeleted)
				return;

			var existing = await _userRoleRepository.GetActiveByUserAndRoleTypeAsync(userId, roleType)
				?? throw new KeyNotFoundException("Role not assigned to the user.");

			existing.IsDeleted = true; // soft delete
			existing.UpdatedAt = DateTime.UtcNow;

			await _userRoleRepository.UpdateAsync(existing);
		}
	}
}