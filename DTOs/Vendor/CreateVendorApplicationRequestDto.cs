using SupplySync.Constants.Enums;
using System.Collections.Generic;

namespace SupplySync.DTOs.Vendor
{
    public class CreateVendorApplicationRequestDto
    {
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
        public List<CreateVendorApplicationDocumentDto>? Documents { get; set; }
    }
}