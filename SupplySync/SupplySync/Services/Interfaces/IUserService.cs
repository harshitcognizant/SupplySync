using SupplySync.DTOs.User;

namespace SupplySync.Services.Interfaces
{
	public interface IUserService
	{
		Task<int> CreateUserAsync(CreateUserRequestDto dto);

		Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserRequestDto dto);


		Task<UserResponseDto> GetUserAsync(int id);
		Task<UserListResponseDto> ListUsersAsync(int pageNumber, int pageSize);

	}
}