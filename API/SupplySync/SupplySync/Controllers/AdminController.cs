using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.API.Interfaces;

namespace SupplySync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
	private readonly IAdminService _adminService;

	public AdminController(IAdminService adminService)
	{
		_adminService = adminService;
	}

	[HttpGet("users")]
	public async Task<IActionResult> GetAllUsers()
	{
		var result = await _adminService.GetAllUsersAsync();
		return Ok(result);
	}

	[HttpPut("users/{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(string id)
	{
		var (found, message, isActive) = await _adminService.ToggleActiveAsync(id);
		if (!found) return NotFound();
		return Ok(new { message, isActive });
	}

	[HttpPut("users/{id}/reset-password")]
	public async Task<IActionResult> ResetPassword(string id, [FromBody] string newPassword)
	{
		var (found, success, message, errors) = await _adminService.ResetPasswordAsync(id, newPassword);
		if (!found) return NotFound();
		if (!success) return BadRequest(errors);
		return Ok(new { message });
	}

	[HttpPut("users/{id}/change-role")]
	public async Task<IActionResult> ChangeRole(string id, [FromBody] string newRole)
	{
		var (found, message) = await _adminService.ChangeRoleAsync(id, newRole);
		if (!found) return NotFound();
		return Ok(new { message });
	}
}