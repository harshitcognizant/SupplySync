using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.User;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}


		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateUserRequestDto dto)
		{
			var id = await _userService.CreateUserAsync(dto);
			return Ok(new { Message = "User created successfully", UserID = id });
		}

		[HttpPut("update/{id:int}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequestDto dto)
		{
			if (id != dto.UserID) return BadRequest(new { Message = "Route id and body UserID must match." });

			var response = await _userService.UpdateUserAsync(id, dto);
			return Ok(response);
		}


		[HttpGet("users/{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await _userService.GetUserAsync(id);
			return Ok(result);
		}


		[HttpGet("users")]
		public async Task<IActionResult> List([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			var result = await _userService.ListUsersAsync(pageNumber, pageSize);
			return Ok(result);
		}


	}
}