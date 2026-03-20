using System.ComponentModel.DataAnnotations;
using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.UserRoles
{
	public class UpdateUserRoleRequestDto
	{

		[Required]
		public RoleType CurrentRoleType { get; set; }

		[Required]
		public RoleType NewRoleType { get; set; }

		public bool? IsDeleted { get; set; }
	}
}