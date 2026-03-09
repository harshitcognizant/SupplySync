using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
	[Table("Vendor")]
	public class Vendor
	{
		[Key]
		public int VendorID { get; set; }

		[Required, MaxLength(150)]
		public string Name { get; set; } = string.Empty;

		[Required, MaxLength(255)]
		public string ContactInfo { get; set; } = string.Empty;

		[Required]
		public VendorCategory Category { get; set; }

		[Required]
		public VendorStatus Status { get; set; }

		[Required]
		public bool IsActive { get; set; } = true;

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

	}
}
