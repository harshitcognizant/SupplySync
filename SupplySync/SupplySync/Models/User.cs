using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
	[Table("User")]
	public class User
	{
		[Key]
		public int UserID { get; set; } // PK, Identity

		[Required, MaxLength(150)]
		public string Name { get; set; } 

		[Required ,MaxLength(100)] 
		public string Email {get; set;}
		
		public string? Phone { get; set; }

        [Required]
        public string Password{ get; set; }

        [Required]
		public UserStatus Status { get; set; }

		[Required]
		public bool IsDeleted { get; set; } 
		public DateTime CreatedAt { get; set; } 
		public DateTime UpdatedAt { get; set; } 
		public ICollection<UserRole> UserRoles { get; set; } 
	}
}