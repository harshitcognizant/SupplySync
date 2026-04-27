using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Notification;
using SupplySync.DTOs.User;
using SupplySync.DTOs.UserRoles;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SupplySync.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;
        private readonly IAuditLogService _auditLogService;
        private readonly INotificationService _notificationService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IUserService _userService;
		private readonly IUserRoleService _userRoleService;
        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IConfiguration config,
            IAuditLogService auditLogService,
            INotificationService notificationService,
			IHttpContextAccessor httpContextAccessor,
			IUserService userService,
			IUserRoleService userRoleService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _config = config;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
			_httpContextAccessor = httpContextAccessor;
			_userService= userService;
			_userRoleService = userRoleService;
		}


        public async Task<int> RegisterVendorApplicantAsync(CreateUserRequestDto dto)
        {
            var userId = await _userService.CreateUserAsync(dto);

            await _userRoleService.AssignRoleToUserAsync(
                userId,
                new CreateUserRoleRequestDto { RoleType = RoleType.VendorApplicant }
            );

            return userId;
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
				userId: user.UserID,               // actor
				userName: user.Name,               // optional
				action: "UserCreated",
				resource: $"User:{user.UserID}"
			);

            await _notificationService.CreateAsync(new CreateNotificationRequestDto
            {
                UserID = user.UserID,
                Message = "User LoggedIn",
                Category = NotificationCategory.System
            });

			var roles = user.UserRoles
 .Where(ur => !ur.IsDeleted && !ur.Role.IsDeleted)
 .Select(ur => ur.Role.RoleType.ToString())
 .Distinct()
 .ToList();

			var accessToken = GenerateAccessToken(user, roles, out var expiry);

			// ✅ ADD REFRESH TOKEN HERE
			var refreshToken = GenerateRefreshToken();
			var refreshExpiry = DateTime.UtcNow.AddDays(7);

			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiresAt = refreshExpiry;
			await _userRepository.UpdateAsync(user);

			// ✅ SET COOKIE
			_httpContextAccessor.HttpContext!.Response.Cookies.Append(
				"refreshToken",
				refreshToken,
				GetRefreshTokenCookieOptions(refreshExpiry)
			);

			return new LoginResponseDto
			{
				Token = accessToken,
				ExpiresAtUtc = expiry,
				UserID = user.UserID,
				Name = user.Name,
				Email = user.Email,
				Roles = roles
			};
		}

		private string GenerateRefreshToken()
		{
			var bytes = RandomNumberGenerator.GetBytes(64);
			return Convert.ToBase64String(bytes);
		}

		private CookieOptions GetRefreshTokenCookieOptions(DateTime expires)
		{
			return new CookieOptions
			{
				HttpOnly = true,
				Secure = true,              // set false only in local dev if needed
				SameSite = SameSiteMode.Strict,
				Expires = expires,
				Path = "/api/auth/refresh"  // cookie sent ONLY to refresh endpoint
			};
		}

		private string GenerateAccessToken(User user, List<string> roles, out DateTime expiresAt)
		{
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
			);
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var issuer = _config["Jwt:Issuer"];
			var audience = _config["Jwt:Audience"];
			var expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

			expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

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
				expires: expiresAt,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<(string Token, DateTime ExpiresAt)> RefreshAsync(string refreshToken)
		{
			if (string.IsNullOrEmpty(refreshToken))
				throw new UnauthorizedAccessException("Refresh token missing.");

			var user = await _userRepository.GetByRefreshTokenAsync(refreshToken)
				?? throw new UnauthorizedAccessException("Invalid refresh token.");

			if (user.RefreshTokenExpiresAt <= DateTime.UtcNow)
				throw new UnauthorizedAccessException("Refresh token expired.");

			// ✅ ROTATE
			var newRefreshToken = GenerateRefreshToken();
			var newExpiry = DateTime.UtcNow.AddDays(7);

			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiresAt = newExpiry;
			await _userRepository.UpdateAsync(user);

			_httpContextAccessor.HttpContext!.Response.Cookies.Append(
				"refreshToken",
				newRefreshToken,
				GetRefreshTokenCookieOptions(newExpiry)
			);

			var roles = user.UserRoles
				.Where(ur => !ur.IsDeleted && !ur.Role.IsDeleted)
				.Select(ur => ur.Role.RoleType.ToString())
				.Distinct()
				.ToList();

			var newAccessToken = GenerateAccessToken(user, roles, out var accessExpiry);

			return (newAccessToken, accessExpiry);
		}

		public async Task LogoutAsync(string refreshToken)
		{
			if (string.IsNullOrEmpty(refreshToken))
				return;

			var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
			if (user == null)
				return;

			// ✅ Revoke refresh token in DB
			user.RefreshToken = null;
			user.RefreshTokenExpiresAt = null;
			await _userRepository.UpdateAsync(user);

			// ✅ Remove cookie from browser
			_httpContextAccessor.HttpContext!.Response.Cookies.Delete("refreshToken");
		}

	}
}