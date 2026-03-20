using System;

namespace SupplySync.DTOs.UserRoles
{
	public class UserRoleResponseDto
	{

		public int UserRoleID { get; set; }
		public int UserID { get; set; }
		public int RoleID { get; set; }
		public string RoleType { get; set; } = default!;
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

	}
}