// /SupplySync/Services/UserService.cs
using System.Diagnostics.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupplySync.DTOs.User;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly IPasswordHasher<User> _passwordHasher;

		public UserService(
			IUserRepository userRepository,
			IMapper mapper,
			IPasswordHasher<User> passwordHasher)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_passwordHasher = passwordHasher;
		}

		// Existing create method (shown for completeness if you have it)
		public async Task<int> CreateUserAsync(CreateUserRequestDto dto)
		{
			var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

			if (await _userRepository.EmailExistsAsync(normalizedEmail))
				throw new InvalidOperationException("Email already in use.");

			var user = _mapper.Map<User>(dto);
			user.Email = normalizedEmail;
			user.Password = _passwordHasher.HashPassword(user, dto.Password);
			var created = await _userRepository.InsertAsync(user);
			return created.UserID;
		}

		// ✅ New: Update
		public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserRequestDto dto)
		{
			var user = await _userRepository.GetByIdAsync(id);
			if (user == null)
				throw new KeyNotFoundException("User not found.");

			// Partial updates: apply only provided fields
			if (dto.Name is not null)
				user.Name = dto.Name;

			if (dto.Email is not null)
			{
				var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

				if (await _userRepository.EmailExistsForOtherAsync(normalizedEmail, id))
					throw new InvalidOperationException("Email already in use.");

				user.Email = normalizedEmail;
			}

			if (dto.Phone is not null)
				user.Phone = dto.Phone;

			if (!string.IsNullOrEmpty(dto.Password))
				user.Password = _passwordHasher.HashPassword(user, dto.Password);

			if (dto.Status.HasValue)
				user.Status = dto.Status.Value;

			if (dto.IsDeleted.HasValue)
				user.IsDeleted = dto.IsDeleted.Value;

			user.UpdatedAt = DateTime.UtcNow;

			await _userRepository.UpdateAsync(user);

			var updatedUser = await _userRepository.GetByIdWithRolesAsync(user.UserID) ?? throw new KeyNotFoundException($"User with ID {user.UserID} was modified but could not be retrieved.");
			return _mapper.Map<UserResponseDto>(updatedUser);
		}


		public async Task<UserResponseDto> GetUserAsync(int id)
		{
			var user = await _userRepository.GetByIdWithRolesAsync(id);
			if (user == null)
				throw new KeyNotFoundException("User not found.");

			return _mapper.Map<UserResponseDto>(user);
		}

		public async Task<UserListResponseDto> ListUsersAsync(int pageNumber, int pageSize)
		{
			var (items, total) = await _userRepository.GetPagedWithRolesAsync(pageNumber, pageSize);

			return new UserListResponseDto
			{
				Items = _mapper.Map<List<UserResponseDto>>(items),
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = total
			};
		}

	}
}
