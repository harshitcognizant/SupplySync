using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;

namespace SupplySync.API.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<object>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.IsActive,
                user.CreatedAt,
                Roles = roles
            });
        }

        return result;
    }

    public async Task<(bool Found, string Message, bool IsActive)> ToggleActiveAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found.", false);

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        var message = $"User {(user.IsActive ? "activated" : "deactivated")} successfully.";
        return (true, message, user.IsActive);
    }

    public async Task<(bool Found, bool Success, string Message, object? Errors)> ResetPasswordAsync(
        string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, false, "User not found.", null);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            return (true, false, "Password reset failed.", result.Errors);

        return (true, true, "Password reset successfully.", null);
    }

    public async Task<(bool Found, string Message)> ChangeRoleAsync(string id, string newRole)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, newRole);

        return (true, $"Role changed to {newRole} successfully.");
    }
}
