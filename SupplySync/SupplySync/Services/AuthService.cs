using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Notification;
using SupplySync.DTOs.User;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly IPasswordHasher<User> _passwordHasher;
		private readonly IConfiguration _config;
		private readonly IAuditLogService _auditLogService;
		private readonly INotificationService _notificationService;

		public AuthService(
			IUserRepository userRepository,
			IPasswordHasher<User> passwordHasher,
			IConfiguration config,
			IAuditLogService auditLogService,
			INotificationService notificationService)
		{
			_userRepository = userRepository;
			_passwordHasher = passwordHasher;
			_config = config;
			_auditLogService = auditLogService;
			_notificationService = notificationService;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
		{
			var email = dto.Email.Trim().ToLowerInvariant();

			var user = await _userRepository.GetByEmailWithRolesAsync(email)
				?? throw new UnauthorizedAccessException("Invalid credentials.");

			if (user.Status == UserStatus.Inactive ||
				user.Status == UserStatus.Suspended ||
				user.Status == UserStatus.Pending)
			{
				throw new UnauthorizedAccessException("User is not active.");
			}

			// Verify password
			var verify = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
			if (verify == PasswordVerificationResult.Failed)
				throw new UnauthorizedAccessException("Invalid credentials.");

			await _auditLogService.WriteAsync(
				userId: user.UserID,               // actor (or null if system)
				userName: user.Name,               // optional (not persisted in model)
				action: "UserCreated",
				resource: $"User:{user.UserID}"
			);

			await _notificationService.CreateAsync(new CreateNotificationRequestDto
			{
				UserID = user.UserID,
				Message = "User LoggedIn",
				Category = NotificationCategory.System
			});

			// Prepare role claims (only non-deleted mappings and roles)
			var roles = user.UserRoles
				.Where(ur => !ur.IsDeleted && !ur.Role.IsDeleted)
				.Select(ur => ur.Role.RoleType.ToString())
				.Distinct()
				.ToList();

			// Build JWT
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var issuer = _config["Jwt:Issuer"];
			var audience = _config["Jwt:Audience"];
			var expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
			};
			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
				signingCredentials: creds);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return new LoginResponseDto
			{
				Token = jwt,
				ExpiresAtUtc = token.ValidTo,
				UserID = user.UserID,
				Name = user.Name,
				Email = user.Email,
				Roles = roles
			};
		}
	}
}