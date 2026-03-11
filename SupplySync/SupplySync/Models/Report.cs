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
        public ReportScope Scope { get; set; } // maps to varchar

        [Required]
        public string Metrics { get; set; }   

        public bool IsDeleted { get; set; }
        public DateTime GeneratedDate { get; set; }  
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
    }

}
