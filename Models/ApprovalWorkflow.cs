using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
    [Table("ApprovalWorkflow")]
    public class ApprovalWorkflow
    {
        [Key]
        public int WorkflowID { get; set; }

        [Required, MaxLength(100)]
        public string Resource { get; set; } = default!; // e.g., "VendorApplication","Invoice"

        [Required]
        public RoleType ApproverRole { get; set; } // which role approves this resource

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}