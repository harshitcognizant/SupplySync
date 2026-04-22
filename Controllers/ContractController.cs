using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Contract;
using SupplySync.Models;
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

		[Authorize]
		[HttpGet("vendor/{vendorId}")]
		public async Task<IActionResult> GetAllContractsByVendorId([FromRoute] int vendorId, [FromQuery] ContractFiltersRequestDto contractFiltersRequestDto) {

			List<ContractWithTermsResponseDto> contractWithTermsResponseDtos = await _contractService.GetAllContractsByVendorId(vendorId, contractFiltersRequestDto);

			return Ok(contractWithTermsResponseDtos);
		}

		[Authorize]
		[HttpGet("{contractId}")]
		public async Task<IActionResult> GetContractById([FromRoute] int contractId)
		{
			ContractResponseDto contractResponseDto = await _contractService.GetContractById(contractId);
			return Ok(contractResponseDto);
		}

        [Authorize(Roles = "ProcurementOfficer,Admin")]
        [HttpPost("")]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractRequestDto dto)
        {
            var result = await _contractService.CreateContract(dto);
            return Ok(result);
        }

        // Allow ProcurementOfficer (and Admin) to update contracts
        [Authorize(Roles = "ProcurementOfficer,Admin")]
        [HttpPut("{contractId}")]
        public async Task<IActionResult> UpdateContract([FromRoute] int contractId, [FromBody] UpdateContractRequestDto updateContractRequestDto)
        {
            var updated = await _contractService.UpdateContract(contractId, updateContractRequestDto);
            return Ok(updated);
        }



        /// <summary>
        /// Contract Terms endpoints
        /// </summary>

        [Authorize(Roles = "ProcurementOfficer,Admin")]
        [HttpPost("{contractId}/terms")]
        public async Task<IActionResult> CreateContractTerm([FromRoute] int contractId, [FromBody] CreateContractTermRequestDto createContractTermRequestDto)
        {
            // ensure the DTO contains contract id
            createContractTermRequestDto.ContractID = contractId;
            var contractTermResponseDto = await _contractService.CreateContractTerm(createContractTermRequestDto);
            return Ok(contractTermResponseDto);
        }

        [Authorize]
		[HttpGet("{contractId}/terms")]
		public async Task<IActionResult> GetAllContractTermByContractId([FromRoute] int contractId,[FromQuery] ContractTermFiltersRequestDto contractTermFiltersRequestDto)
		{
			List<ContractTermResponseDto> contractResponseDtos = await _contractService.GetAllContractTermByContractId(contractId, contractTermFiltersRequestDto);
			return Ok(contractResponseDtos);
		}    

        
    }
}
