using Microsoft.AspNetCore.Mvc;
using SupplySync.API.DTOs.Auth;
using SupplySync.API.Interfaces;

namespace SupplySync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	// Admin creates internal users
	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterDto dto)
	{
		var (success, message, errors) = await _authService.RegisterAsync(dto);
		if (!success) return BadRequest(errors);
		return Ok(new { message });
	}

	// Vendor self-registration
	[HttpPost("vendor-register")]
	public async Task<IActionResult> VendorRegister([FromBody] VendorRegisterDto dto)
	{
		var (success, message, errors) = await _authService.VendorRegisterAsync(dto);
		if (!success) return BadRequest(errors);
		return Ok(new { message });
	}

	// Login for all roles
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDto dto)
	{
		var (success, message, data) = await _authService.LoginAsync(dto);
		if (!success) return Unauthorized(new { message });
		return Ok(data);
	}
}