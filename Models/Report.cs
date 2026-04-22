using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{

    [Table("Report")]
    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        public ReportScope Scope { get; set; }

        [Required]
        public string Metrics { get; set; } = string.Empty; // Initialized to avoid nulls

        public bool IsDeleted { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

}
