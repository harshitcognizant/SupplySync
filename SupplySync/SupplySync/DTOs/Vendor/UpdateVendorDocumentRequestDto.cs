using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
	public class UpdateVendorDocumentRequestDto
	{

		public VendorDocumentDocType? DocType { get; set; }
		public IFormFile? DocFile { get; set; }
	}
}
