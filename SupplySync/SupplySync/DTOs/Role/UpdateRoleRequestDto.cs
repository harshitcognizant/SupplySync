using System.ComponentModel.DataAnnotations;
using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Role
{
	public class UpdateRoleRequestDto
	{
		[Required]
		public int RoleID { get; set; }

		public RoleType? RoleType { get; set; }

		public bool? IsDeleted { get; set; }
	}
}