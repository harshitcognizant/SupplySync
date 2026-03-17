using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Report
{
    public class ReportResponseDto
    {

        public int ReportID { get; set; }
        public ReportScope Scope { get; set; }
        public string Metrics { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
