using System.ComponentModel.DataAnnotations;
using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.User
{

		public class UpdateUserRequestDto
		{
			[Required]
			public int UserID { get; set; }

			[MaxLength(150)]
			public string? Name { get; set; }

			[EmailAddress, MaxLength(100)]
			public string? Email { get; set; }

			[Phone]
			public string? Phone { get; set; }

			[MinLength(8), MaxLength(255)]
			[DataType(DataType.Password)]
			public string? Password { get; set; }

			public UserStatus? Status { get; set; }
			public bool? IsDeleted { get; set; }
		}
	
}
