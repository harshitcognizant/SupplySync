using SupplySync.DTOs.User;

namespace SupplySync.Services.Interfaces
{
	public interface IAuthService
	{
		Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
	}
}
