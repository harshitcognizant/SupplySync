namespace SupplySync.DTOs.Compliance
{
	public class ComplianceRecordListResponseDto
	{
		public int ComplianceID { get; set; }
		public int ContractID { get; set; }
		public string Type { get; set; } = string.Empty;
		public string Result { get; set; } = string.Empty;
		public DateOnly Date { get; set; }
		public string? Notes { get; set; }
	}
}