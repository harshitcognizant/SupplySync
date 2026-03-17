

namespace SupplySync.DTOs.Audit
{
    public class CreateAuditRequestDto
    {
        public int ComplianceOfficerID { get; set; }
        public string Scope { get; set; } = string.Empty;
        public string? Findings { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;

    }
}
