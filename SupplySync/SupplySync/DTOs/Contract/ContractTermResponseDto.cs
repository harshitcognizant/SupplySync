namespace SupplySync.DTOs.Contract
{
	public class ContractTermResponseDto
	{
		public int TermID { get; set; }
		public string Description { get; set; }
		public bool ComplianceFlag { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
