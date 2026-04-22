using SupplySync.Constants.Enums;
using System;
using System.Collections.Generic;

namespace SupplySync.DTOs.Vendor
{
    public class VendorApplicationResponseDto
    {
        public int ApplicationID { get; set; }
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
        public VendorStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<VendorDocumentResponseDto>? Documents { get; set; }
    }
}