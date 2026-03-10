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

        [ForeignKey("POID")]
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

        [Required]
        public int VendorID { get; set; }

        [ForeignKey("VendorID")]
        public virtual Vendor Vendor { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(150)]
        public string Item { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Shipped;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}