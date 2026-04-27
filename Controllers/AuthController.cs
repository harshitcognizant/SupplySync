// /SupplySync/Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.User;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

        [AllowAnonymous]
        [HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
		{
				var result = await _authService.LoginAsync(dto);
				return Ok(result);
		}
		[HttpPost("refresh")]
		[AllowAnonymous]
		public async Task<IActionResult> Refresh()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			var result = await _authService.RefreshAsync(refreshToken!);

			return Ok(new
			{
				Token = result.Token,
				ExpiresAtUtc = result.ExpiresAt
			});
		}

		[AllowAnonymous]
		[HttpPost("vendor-signup")]
		public async Task<IActionResult> VendorSignup([FromBody] CreateUserRequestDto dto)
		{
			dto.Status = UserStatus.Active;

			var userId = await _authService.RegisterVendorApplicantAsync(dto);
			
			return Ok(new
			{
				Message = "vendor signup successfull , Please apply for vendor approval",
				UserId = userId

            }
				);
        }



        [HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			await _authService.LogoutAsync(refreshToken!);

			return Ok(new { Message = "Logged out successfully." });
		}
	}
}
