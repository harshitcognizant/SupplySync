namespace SupplySync.DTOs.Admin {
    public class CreateVendorCategoryRequestDto {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true; 
    }
}