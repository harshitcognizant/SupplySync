using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
    public class GetVendorFiltersRequestDto
    {
        public string? Name { get; set; }
        public VendorCategory? Category { get; set; }
        public VendorStatus? Status { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
