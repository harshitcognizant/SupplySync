using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{

    [Table("Audit")]
    public class Audit
    {
        [Key]
        public int AuditID { get; set; }

        [Required]
        public int ComplianceOfficerID { get; set; } // FK → User.UserID

        [Required, MaxLength(200)]
        public string Scope { get; set; } // varchar

        public string? Findings { get; set; } // text

        [Required]
        public DateOnly Date { get; set; } // DATE

        [Required]
         public AuditStatus Status { get; set; } // maps to varchar

        public bool IsDeleted { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User ComplianceOfficer { get; set; } = default!;
    }
}
