// /SupplySync/Security/ClaimsPrincipalExtensions.cs
using System.Security.Claims;

namespace SupplySync.Security
{
	public static class ClaimsPrincipalExtensions
	{
		public static int? GetUserId(this ClaimsPrincipal user)
		{
			var id = user.FindFirstValue(ClaimTypes.NameIdentifier)
				  ?? user.FindFirstValue(ClaimTypes.Name);
			return int.TryParse(id, out var uid) ? uid : (int?)null;
		}

		public static bool IsInRoleAny(this ClaimsPrincipal user, params string[] roles)
			=> roles.Any(user.IsInRole);
	}
}