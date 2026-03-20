using System.Collections.Generic;

namespace SupplySync.DTOs.UserRoles
{
	public class UserRoleListResponseDto
	{
		public List<UserRoleResponseDto> Items { get; set; } = new();

		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
	}
}