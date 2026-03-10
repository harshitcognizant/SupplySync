using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
	[Table("Contract")]
	public class Contract
	{
		[Key]
		public int ContractID { get; set; }

		[Required]
		public int VendorID {  get; set; }

		[ForeignKey("VendorID")]
		public virtual Vendor Vendor { get; set; } = null!;

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }

		[Required]
		[Column(TypeName = "decimal(18,2)")]
		public decimal Value {  get; set; }

		[Required]
		public ContractStatus Status { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

	}
}
