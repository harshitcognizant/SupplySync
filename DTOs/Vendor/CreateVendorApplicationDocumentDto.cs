using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
    public class CreateVendorApplicationDocumentDto
    {
        public VendorDocumentDocType DocType { get; set; }
        public string FileURI { get; set; } = default!;
    }
}