using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Contract;
using SupplySync.Services;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContractController : ControllerBase
	{
		private readonly IContractService _contractService;

		public ContractController(IContractService contractService)
		{
			_contractService = contractService;
		}
		/// <summary>
		/// Contract endpoints
		/// </summary>


		[HttpGet("{contractId}")]
		public async Task<IActionResult> GetContractById([FromRoute] int contractId)
		{
			ContractResponseDto contractResponseDto = await _contractService.GetContractById(contractId);
			return Ok(contractResponseDto);
		}

		[HttpPost("")]
		public async Task<IActionResult> CreateContract([FromBody] CreateContractRequestDto createContractRequestDto)
		{
			ContractResponseDto contractResponseDto = await _contractService.CreateContract(createContractRequestDto);
			return Ok(contractResponseDto);
		}

		[HttpPut("{contractId}")]
		public async Task<IActionResult> UpdateContract([FromRoute] int contractId, UpdateContractRequestDto updateContractRequestDto)
		{
			ContractResponseDto contractResponseDto = await _contractService.UpdateContract(contractId ,updateContractRequestDto);
			return Ok();
		}



		/// <summary>
		/// Contract Terms endpoints
		/// </summary>

		[HttpPost("{contractId}/terms")]
		public async Task<IActionResult> CreateContractTerm([FromBody] CreateContractTermRequestDto createContractTermRequestDto)
		{
			ContractTermResponseDto contractTermResponseDto = await _contractService.CreateContractTerm(createContractTermRequestDto);
			return Ok(contractTermResponseDto);
		}
	}
}
