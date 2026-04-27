using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Vendor;
using SupplySync.Security;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly IVendorApplicationService _vendorApplicationService;

        public VendorController(IVendorService vendorService, IVendorApplicationService vendorApplicationService)
        {
            _vendorService = vendorService;
            _vendorApplicationService = vendorApplicationService;
        }


        [Authorize(Roles = "VendorApplicant")]
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] CreateVendorApplicationRequestDto dto)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            dto.UserID = userId.Value;

            var created = await _vendorApplicationService.CreateApplicationAsync(dto);
            return Ok(created);
        }

        // Existing endpoints kept as-is...
        [Authorize]
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetVendorById([FromRoute] int vendorId)
        {
            var dto = await _vendorService.GetVendorById(vendorId);
            return Ok(dto);
        }

        [Authorize(Roles = "Admin,ProcurementOfficer,WarehouseManager,FinanceOfficer,ComplianceOfficer")]
        [HttpGet("")]
        public async Task<IActionResult> GetAllVendorWithFilter([FromQuery] GetVendorFiltersRequestDto getVendorFiltersRequestDto)
        {
            var list = await _vendorService.GetAllVendorWithFilter(getVendorFiltersRequestDto);
            return Ok(list);
        }

        //[Authorize(Roles = "Admin")]
        //[HttpPost("")]
        //public async Task<IActionResult> CreateVendor([FromBody] CreateVendorApplicationDocumentDto createVendorRequestDto)
        //{
        //    var created = await _vendorService.CreateVendor(createVendorRequestDto);
        //    return Ok(created);
        //}

        [Authorize(Roles = "Admin,VendorUser")]
        [HttpPut("{vendorId}")]
        public async Task<IActionResult> UpdateVendor([FromRoute] int vendorId, [FromBody] UpdateVendorRequestDto updateVendorRequestDto)
        {
            var updated = await _vendorService.UpdateVendor(vendorId, updateVendorRequestDto);
            return Ok(updated);
        }

        // Documents endpoints omitted for brevity (keep existing)...

        // -------------------------
        // Vendor application endpoints
        // -------------------------

        /// <summary>
        /// Vendor self-signup: anonymous endpoint to apply.
        /// </summary>
        //[AllowAnonymous]
        //[HttpPost("apply")]
        //public async Task<IActionResult> Apply([FromBody] CreateVendorApplicationRequestDto dto)
        //{
        //    var created = await _vendorApplicationService.CreateApplicationAsync(dto);
        //    return Ok(created);
        //}

        [Authorize(Roles = "ProcurementOfficer,Admin")]
        [HttpGet("applications/{id}")]
        public async Task<IActionResult> GetApplication([FromRoute] int id)
        {
            var dto = await _vendorApplicationService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Approve vendor application (ProcurementOfficer or Admin)
        /// </summary>
        [Authorize(Roles = "ProcurementOfficer,Admin")]
        [HttpPost("applications/{id}/approve")]
        public async Task<IActionResult> ApproveApplication([FromRoute] int id)
        {
            var result = await _vendorApplicationService.ApproveApplicationAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Reject vendor application (ProcurementOfficer or Admin)
        /// </summary>
        [Authorize(Roles = "ProcurementOfficer,Admin")]
        [HttpPost("applications/{id}/reject")]
        public async Task<IActionResult> RejectApplication([FromRoute] int id, [FromBody] string reason)
        {
            var ok = await _vendorApplicationService.RejectApplicationAsync(id, reason);
            if (!ok) return NotFound();
            return Ok();
        }
    }
}