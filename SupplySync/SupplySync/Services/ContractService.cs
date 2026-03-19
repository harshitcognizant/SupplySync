using System.Numerics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Contract;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
	public class ContractService : IContractService
	{
		private readonly IContractRepository _contractRepository;
		private readonly IMapper _mapper;

		public ContractService(IContractRepository contractRepository, IMapper mapper)
		{
			_contractRepository = contractRepository;
			_mapper = mapper;
		}

		[HttpGet("{contractId}")]
		public async Task<ContractResponseDto> GetContractById(int contractId)
		{
			Contract? contract = await _contractRepository.GetContractById(contractId);
			if (contract == null) {
				throw new KeyNotFoundException("Contract Not Found");
			}
			return _mapper.Map<ContractResponseDto>(contract);
		}
		public async Task<ContractResponseDto> CreateContract(CreateContractRequestDto createContractRequestDto)
		{
			Contract newContract = _mapper.Map<Contract>(createContractRequestDto);
			
			Contract contract = await _contractRepository.CreateContract(newContract);
			if (contract == null)
			{
				throw new ArgumentException("Contract Not Created, some error occured");
			}
			ContractResponseDto contractResponseDto = _mapper.Map<ContractResponseDto>(contract);

			return contractResponseDto;
		}

		public async Task<ContractResponseDto> UpdateContract(int contractId, UpdateContractRequestDto updateContractRequestDto)
		{

			Contract? existingContract = await _contractRepository.GetContractById(contractId);

			if (existingContract == null)
			{
				throw new KeyNotFoundException($"Vendor with ID {contractId} not found.");
			}

			_mapper.Map(updateContractRequestDto, existingContract);
			existingContract.ContractID = contractId;
			Contract? updatedContract = await _contractRepository.UpdateContract(existingContract);

			if (updatedContract != null) {
				throw new KeyNotFoundException("Contract Data not Updated.");
			}

			ContractResponseDto contractResponseDto = _mapper.Map<ContractResponseDto>(existingContract);

			return contractResponseDto;
		}

		public async Task<ContractTermResponseDto> CreateContractTerm(CreateContractTermRequestDto createContractTermRequestDto)
		{
			ContractTerm newContractTerm = _mapper.Map<ContractTerm>(createContractTermRequestDto);

			ContractTerm? contractTerm = await _contractRepository.CreateContractTerm(newContractTerm);
			if (contractTerm == null)
			{
				throw new ArgumentException("Contract Term Not Created, some error occured");
			}
			ContractTermResponseDto contractTermResponseDto = _mapper.Map<ContractTermResponseDto>(contractTerm);
			return contractTermResponseDto;
		}
	}
}
