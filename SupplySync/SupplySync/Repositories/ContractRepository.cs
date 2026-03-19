using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
	public class ContractRepository : IContractRepository
	{
		private readonly AppDbContext _appDbContext;
		public ContractRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public async Task<Contract?> GetContractById(int contractId)
		{
			Contract? contract =  await _appDbContext.Contracts.FirstOrDefaultAsync(x => x.ContractID == contractId);
			return contract;
		}

		public async Task<Contract> CreateContract(Contract newContract)
		{
			await _appDbContext.Contracts.AddAsync(newContract);
			await _appDbContext.SaveChangesAsync();
			return newContract;
		}

		public async Task<Contract?> UpdateContract(Contract contract)
		{
			_appDbContext.Contracts.Update(contract);
			await _appDbContext.SaveChangesAsync();
			return contract;
		}

		public async Task<ContractTerm> CreateContractTerm(ContractTerm newContractTerm)
		{
			await _appDbContext.ContractTerms.AddAsync(newContractTerm);
			await _appDbContext.SaveChangesAsync();
			return newContractTerm;
		}

		
	}
}
