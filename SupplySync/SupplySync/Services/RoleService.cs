// /SupplySync/Services/RoleService.cs
using AutoMapper;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Role;
using SupplySync.DTOs.User;
using SupplySync.Models;
using SupplySync.Repositories;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
	public class RoleService : IRoleService
	{
		private readonly IRoleRepository _roleRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;

		public RoleService(IRoleRepository roleRepository, IMapper mapper, IUserRepository userRepository)
		{
			_roleRepository = roleRepository;
			_mapper = mapper;
			_userRepository = userRepository;
		}

		public async Task<RoleListResponseDto> ListRolesAsync(int pageNumber, int pageSize)
		{
			var (items, total) = await _roleRepository.GetPagedAsync(pageNumber, pageSize);
			return new RoleListResponseDto
			{
				Items = _mapper.Map<List<RoleResponseDto>>(items),
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = total
			};
		}

		public async Task<UserListResponseDto> ListUsersByRoleAsync(RoleType roleType, int pageNumber, int pageSize)
		{
			var (items, total) = await _userRepository.GetUsersByRoleAsync(roleType, pageNumber, pageSize);
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