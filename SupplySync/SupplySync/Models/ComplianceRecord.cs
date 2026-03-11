using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{

    [Table("ComplianceRecord")]
    public class ComplianceRecord
    {

        [Key]
        public int ComplianceID { get; set; }

        [Required]
        public int ContractID { get; set; } // FK → Contract.ContractID

        [Required]
        public ComplianceType Type { get; set; }   // maps to varchar via converter

        [Required]
        public ComplianceResult Result { get; set; } // maps to varchar via converter

        [Required]
        public DateOnly Date { get; set; } // maps to DATE

        public string? Notes { get; set; } // text
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual Contract Contract { get; set; } 
    }

}
