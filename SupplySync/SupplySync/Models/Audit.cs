using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants;

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
        public string Scope { get; set; } = default!; // varchar

        public string? Findings { get; set; } // text

        [Required]
        public DateOnly Date { get; set; } // DATE

        [Required]
        public AuditStatus Status { get; set; } // maps to varchar

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(ComplianceOfficerID))]
        public virtual User ComplianceOfficer { get; set; } = default!;
    }
}
