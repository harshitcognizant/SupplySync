using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.User
{

		public class UserResponseDto
		{
			public int UserID { get; set; }

			public string Name { get; set; } = default!;

			public string Email { get; set; } = default!;

			public string? Phone { get; set; }

			public UserStatus Status { get; set; }

			public bool IsDeleted { get; set; }

			public DateTime CreatedAt { get; set; }

			public DateTime UpdatedAt { get; set; }

			public List<UserRoleSummaryDto> Roles { get; set; } = new();
		}

		public class UserRoleSummaryDto
		{
			public int RoleID { get; set; }
			public string RoleType { get; set; } = default!;
		}
	
}
