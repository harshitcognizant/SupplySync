using SupplySync.API.DTOs.Auth;

namespace SupplySync.API.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, object? Errors)> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string Message, object? Errors)> VendorRegisterAsync(VendorRegisterDto dto);
    Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginDto dto);
}
