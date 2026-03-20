using System.ComponentModel.DataAnnotations;

namespace SupplySync.DTOs.User
{

	public class LoginRequestDto
	{
		[Required, EmailAddress, MaxLength(100)]
		public string Email { get; set; } = default!;

		[Required, MinLength(8)]
		public string Password { get; set; } = default!;
	}
}
