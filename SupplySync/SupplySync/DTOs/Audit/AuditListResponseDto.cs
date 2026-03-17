using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Audit
{
    public class AuditListResponseDto
    {

        public int AuditID { get; set; }
        public int ComplianceOfficerID { get; set; }
        public string Scope { get; set; } = string.Empty;
        public AuditStatus Status { get; set; }
        public DateOnly Date { get; set; }

    }
}
