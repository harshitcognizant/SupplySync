using SupplySync.DTOs.User;

namespace SupplySync.Services.Interfaces
{
	public interface IAuthService
	{
        Task<int> RegisterVendorApplicantAsync(CreateUserRequestDto dto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
		Task LogoutAsync(string refreshToken);
		Task<(string Token, DateTime ExpiresAt)> RefreshAsync(string refreshToken);
	}
}
