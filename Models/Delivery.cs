using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
    [Table("Delivery")]
    public class Delivery
    {
        [Key]
        public int DeliveryID { get; set; }

        [Required]
        public int POID { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; set; } 

        [Required]
        public int VendorID { get; set; }

        public virtual Vendor Vendor { get; set; } 

        [Required]
        public DateTime Date { get; set; }

        // legacy single-item fields kept for compatibility
        public string? Item { get; set; } 
        public int? Quantity { get; set; }

        // New: JSON payload of items (array of { item, quantity })
        public string? ItemsJson { get; set; }

        public bool IsDeleted { get; set; }
        public DeliveryStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; } 
    }
}