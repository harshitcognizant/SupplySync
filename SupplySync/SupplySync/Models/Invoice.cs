using SupplySync.Constants.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;

namespace SupplySync.Models
{
    [Table("Invoice")]
    public class Invoice
    {
        
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public int POID { get; set; }
 
         public virtual PurchaseOrder PurchaseOrder { get; set; } 

        [Required]
        public int VendorId { get; set; }
 
        public virtual Vendor Vendor { get; set; } 

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get;set;  }

        [Required]
        public InvoiceStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; } 

        public bool IsDeleted { get; set; }

    }
}
