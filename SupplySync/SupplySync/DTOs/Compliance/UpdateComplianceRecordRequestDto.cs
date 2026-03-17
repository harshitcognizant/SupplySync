using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Compliance
{
    public class UpdateComplianceRecordRequestDto
    {

        public ComplianceType Type { get; set; }
        public ComplianceResult Result { get; set; }
        public DateOnly Date { get; set; }
        public string? Notes { get; set; }

    }
}
