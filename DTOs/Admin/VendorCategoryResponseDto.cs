namespace SupplySync.DTOs.Admin { 
    public class VendorCategoryResponseDto {
        public int VendorCategoryID { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

