using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Audit
{
    public class AuditResponseDto
    {

        public int AuditID { get; set; }
        public int ComplianceOfficerID { get; set; }
        public string Scope { get; set; } = string.Empty;
        public string? Findings { get; set; }
        public DateOnly Date { get; set; }
        public AuditStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
