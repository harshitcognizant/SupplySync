
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
	[Table("UserRole")]
	public class UserRole
	{
		[Key]
        public int UserRoleID {  get; set; }

        [Required]
		public int UserID { get; set; }

		[Required]
		public int RoleID { get; set; } 
		public DateTime CreatedAt { get; set; }  
		public DateTime UpdatedAt { get; set; } 

		public bool IsDeleted { get; set; }

        // Navigation
        public virtual User User { get; set; } 
		public virtual Role Role { get; set; } 
	}
}