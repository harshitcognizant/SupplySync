namespace SupplySync.DTOs.Contract
{
	public class CreateContractTermRequestDto
	{
		public int ContractID { get; set; }
		public string Description { get; set; }
		public bool ComplianceFlag { get; set; }
	}
}
