using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
	public class VendorDocumentResponseDto
	{
		public int DocumentID { get; set; }
		public VendorDocumentDocType DocType { get; set; }
		public string FileURI { get; set; }
		public DateTime UploadedDate { get; set; }
		public VendorDocumentVerificationStatus VerificationStatus { get; set; }
	}
}
