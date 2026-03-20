using System;

using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Role
{
	public class RoleResponseDto
	{
		public int RoleID { get; set; }

		public RoleType RoleType { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}