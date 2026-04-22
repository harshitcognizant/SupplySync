using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
    public class VendorResponseDto
    {
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
        public VendorStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
