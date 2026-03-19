using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
	public class CreateVendorDocumentRequestDto
	{
		public int? VendorID { get; set; }
		public VendorDocumentDocType DocType { get; set; }
		public IFormFile? DocFile { get; set; }

	}
}
