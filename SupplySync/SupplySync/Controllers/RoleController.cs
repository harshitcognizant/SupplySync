// /SupplySync/Controllers/RoleController.cs
using Microsoft.AspNetCore.Mvc;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Role;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RoleController : ControllerBase
	{
		private readonly IRoleService _roleService;
		public RoleController(IRoleService roleService) => _roleService = roleService;

		// GET /roles?pageNumber=1&pageSize=10
		[HttpGet("roles")]
		public async Task<IActionResult> List([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			var result = await _roleService.ListRolesAsync(pageNumber, pageSize);
			return Ok(result);
		}


		[HttpGet("roles/{roleType}/users")]
		public async Task<IActionResult> ListUsersByRole([FromRoute] string roleType, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			if (!Enum.TryParse<RoleType>(roleType, ignoreCase: true, out var parsed)) return BadRequest(new { Message = $"Invalid roleType '{roleType}'." });

			var result = await _roleService.ListUsersByRoleAsync(parsed, pageNumber, pageSize);
			return Ok(result);
		}

	}
}