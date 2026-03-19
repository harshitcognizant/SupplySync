using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
	public interface IContractRepository
	{
		Task<Contract> CreateContract(Contract newContract);
		Task<ContractTerm> CreateContractTerm(ContractTerm newContractTerm);
		Task<Contract> GetContractById(int contractId);
		Task<Contract?> UpdateContract(Contract contract);
	}
}
