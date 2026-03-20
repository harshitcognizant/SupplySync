using System.ComponentModel.DataAnnotations;
using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.UserRoles
{
	public class CreateUserRoleRequestDto
	{
		[Required]
		public RoleType RoleType { get; set; }
	}
}