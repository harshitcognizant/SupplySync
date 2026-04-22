using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{

    [Table("ContractTerm")]
    public class ContractTerm
    {
        [Key]
        public int TermID { get; set; }

        [Required]
        public int ContractID { get; set; }

        public virtual Contract Contract { get; set; }

        // Short description (delivery / quality / penalty summary)
        [Required]
        public string Description { get; set; } = default!;

        // Compliance flag (existing)
        [Required]
        public bool ComplianceFlag { get; set; }

        // New fields for contract terms
        // Delivery timeline in days (nullable)
        public int? DeliveryTimelineDays { get; set; }

        // Quality requirements (free text)
        public string? QualityRequirement { get; set; }

        // Penalty clauses (free text)
        public string? PenaltyClause { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}