using Microsoft.AspNetCore.Components.Web;
using SupplySync.Constants.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplySync.Models
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } 

        [Required]
        public decimal Amount { get; set;  }

        [Required]
        public DateTime Date { get; set; } 
        [Required]
        public PaymentMethod Method { get; set;  }
        [Required]
        public PaymentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }


    }
}
