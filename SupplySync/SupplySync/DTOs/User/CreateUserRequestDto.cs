using System.ComponentModel.DataAnnotations;
using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.User
{
	public class CreateUserRequestDto
	{
		[Required, MaxLength(150)]
		public string Name { get; set; } = default!;

		[Required, EmailAddress, MaxLength(100)]
		public string Email { get; set; } = default!;

		[Phone]
		public string? Phone { get; set; }

		[Required, MinLength(8)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = default!;

		[Required]
		public UserStatus Status { get; set; } = UserStatus.Pending;
	}
}
	
