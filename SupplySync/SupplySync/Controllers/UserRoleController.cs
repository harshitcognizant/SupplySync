// /SupplySync/Controllers/UserRoleController.cs
using Microsoft.AspNetCore.Mvc;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.UserRoles;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserRoleController : ControllerBase
	{
		private readonly IUserRoleService _userRoleService;
		public UserRoleController(IUserRoleService userRoleService) => _userRoleService = userRoleService;

		[HttpPost("users/{id:int}/roles")]
		public async Task<IActionResult> Assign(int id, [FromBody] CreateUserRoleRequestDto dto)
		{
			var result = await _userRoleService.AssignRoleToUserAsync(id, dto);
			return Ok(result);
		}

		[HttpGet("users/{id:int}/roles")]
		public async Task<IActionResult> List(int id)
		{
			var result = await _userRoleService.GetRolesForUserAsync(id);
			return Ok(result);
		}

		[HttpPut("users/{id:int}/roles")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRoleRequestDto dto)
		{
			var result = await _userRoleService.UpdateUserRoleAsync(id, dto);
			return Ok(result);
		}

		[HttpDelete("users/{id:int}/roles/{roleType}")]
		public async Task<IActionResult> Remove(int id, string roleType)
		{
			if (!Enum.TryParse<RoleType>(roleType, ignoreCase: true, out var parsed)) return BadRequest(new { Message = $"Invalid roleType '{roleType}'." });

			await _userRoleService.RemoveRoleFromUserAsync(id, parsed);
			return NoContent();
		}
	}
}
