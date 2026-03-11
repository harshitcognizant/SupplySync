
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

        public virtual Contract Contract { get; set; } 

        [Required]
        public DateTime Date { get; set; }

        [Required] 
        public string Item { get; set; } 

        [Required]
        public int Quantity { get; set; }

        [Required]
        public POStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }        
        public virtual ICollection<Delivery> Deliveries { get; set; } 
    }
}