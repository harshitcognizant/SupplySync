
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
    [Table("PurchaseOrder")]
    public class PurchaseOrder
    {
        [Key]
        public int POID { get; set; }

        [Required]
        public int ContractID { get; set; }

        [ForeignKey("ContractID")]
        public virtual Contract Contract { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(150)]
        public string Item { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public POStatus Status { get; set; } = POStatus.Draft;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

       
        public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}