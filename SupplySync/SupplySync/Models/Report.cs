using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants;

namespace SupplySync.Models
{

    [Table("Report")]
    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        public ReportScope Scope { get; set; } // maps to varchar

        [Required]
        public string Metrics { get; set; } = "{}";

        [Required]
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow; // timestamp

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

}
