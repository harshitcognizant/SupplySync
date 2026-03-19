using SupplySync.DTOs.Contract;

namespace SupplySync.Services.Interfaces
{
	public interface IContractService
	{
		Task<ContractResponseDto> CreateContract(CreateContractRequestDto createContractRequestDto);
		Task<ContractTermResponseDto> CreateContractTerm(CreateContractTermRequestDto createContractTermRequestDto);
		Task<ContractResponseDto> GetContractById(int contractId);
		Task<ContractResponseDto> UpdateContract(int contractId, UpdateContractRequestDto updateContractRequestDto);
	}
}
