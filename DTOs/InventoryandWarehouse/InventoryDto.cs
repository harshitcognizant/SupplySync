using SupplySync.Constants.Enums;
using System.ComponentModel.DataAnnotations;

namespace SupplySync.DTOs.InventoryandWarehouse
{

    public class IssueInventoryRequestDto
    {
        public int WarehouseID { get; set; }
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Reason { get; set; }
    }

    public class CreateInventoryRequestDto
    {
        [Required]
        public int WarehouseID { get; set; }

        [Required]
        public string Item { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateOnly DateAdded { get; set; }

        [Required]
        public InventoryStatus Status { get; set; }
    }
    public class InventoryListResponseDto
    {
        public int InventoryID { get; set; }
        public string Item { get; set; }
        public int Quantity { get; set; }
        public DateOnly DateAdded { get; set; }
        public InventoryStatus Status { get; set; }
    }
    public class InventoryResponseDto
    {
        public int InventoryID { get; set; }
        public int WarehouseID { get; set; }
        public string Item { get; set; }
        public int Quantity { get; set; }
        public DateOnly DateAdded { get; set; }
        public InventoryStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class UpdateInventoryRequestDto
    {
        [Required]
        public int InventoryID { get; set; }

        public int? WarehouseID { get; set; }

        public string? Item { get; set; }

        public int? Quantity { get; set; }

        public DateOnly? DateAdded { get; set; }

        public InventoryStatus? Status { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
