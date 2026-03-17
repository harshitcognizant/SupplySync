using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Compliance
{
    public class ComplianceRecordListResponseDto
    {

        public int ComplianceID { get; set; }
        public int ContractID { get; set; }
        public ComplianceType Type { get; set; }
        public ComplianceResult Result { get; set; }
        public DateOnly Date { get; set; }
        public string? Notes { get; set; }

    }
}
