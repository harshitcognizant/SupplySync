namespace SupplySync.API.Interfaces;

public interface IAdminService
{
    Task<List<object>> GetAllUsersAsync();
    Task<(bool Found, string Message, bool IsActive)> ToggleActiveAsync(string id);
    Task<(bool Found, bool Success, string Message, object? Errors)> ResetPasswordAsync(string id, string newPassword);
    Task<(bool Found, string Message)> ChangeRoleAsync(string id, string newRole);
}
