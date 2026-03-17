using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Report
{
    public class CreateReportRequestDto
    {

        public ReportScope Scope { get; set; }
        public string Metrics { get; set; } = string.Empty;

    }
}
