using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using SupplySync.Constants;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
	[Table("VendorDocument")]
	public class VendorDocument
	{
		[Key]
		public int DocumentID { get; set; }

		[Required]
		public int VendorID { get; set; }

		[Required]
		public VendorDocumentDocType DocType { get; set; }

		[Required]
		public string FileURI { get; set; } = string.Empty;

		[Required]
		public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

		[Required]
		public VendorDocumentVerificationStatus VerificationStatus { get; set; } = VendorDocumentVerificationStatus.Pending;
		
		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;


		[ForeignKey("VendorID")]
		public virtual Vendor Vendor { get; set; } = null!;

	}
}
