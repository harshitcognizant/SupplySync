// /SupplySync/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.User;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService) => _authService = authService;

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
		{
				var result = await _authService.LoginAsync(dto);
				return Ok(result);
		}
	}
}
