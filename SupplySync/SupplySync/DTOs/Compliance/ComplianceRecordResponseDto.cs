using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Compliance
{
    public class ComplianceRecordResponseDto
    {

        public int ComplianceID { get; set; }
        public int ContractID { get; set; }
        public ComplianceType Type { get; set; }
        public ComplianceResult Result { get; set; }
        public DateOnly Date { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
