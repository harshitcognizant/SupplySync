using System.Collections.Generic;
using SupplySync.DTOs.Role;

namespace SupplySync.DTOs.Role
{
	public class RoleListResponseDto
	{
		public List<RoleResponseDto> Items { get; set; } = new();

		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
	}
}