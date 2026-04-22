using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
    public class UpdateVendorRequestDto
    {
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
    }
}