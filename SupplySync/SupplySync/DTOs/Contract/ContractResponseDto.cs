using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Contract
{
	public class ContractResponseDto
	{
		public int ContractID { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public decimal Value { get; set; }
		public ContractStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
