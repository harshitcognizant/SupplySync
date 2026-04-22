using SupplySync.DTOs.Contract;

namespace SupplySync.Services.Interfaces
{
	public interface IContractService
	{
		Task<ContractResponseDto> CreateContract(CreateContractRequestDto createContractRequestDto); 
		Task<ContractTermResponseDto> CreateContractTerm(CreateContractTermRequestDto createContractTermRequestDto);
		Task<List<ContractWithTermsResponseDto>> GetAllContractsByVendorId(int vendorId, ContractFiltersRequestDto contractFiltersRequestDto);
		Task<List<ContractTermResponseDto>> GetAllContractTermByContractId(int contractId, ContractTermFiltersRequestDto contractTermFiltersRequestDto);
		Task<ContractResponseDto> GetContractById(int contractId);
		Task<ContractResponseDto> UpdateContract(int contractId, UpdateContractRequestDto updateContractRequestDto);
	}
}
