using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Report
{
    public class UpdateReportRequestDto
    {

        public ReportScope Scope { get; set; }
        public string Metrics { get; set; } = string.Empty;

    }
}
