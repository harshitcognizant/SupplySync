using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Compliance;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplianceController : ControllerBase
    {
        private readonly IComplianceRecordService _service;

        public ComplianceController(IComplianceRecordService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComplianceRecordRequestDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { Message = "Compliance record created", ComplianceID = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateComplianceRecordRequestDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(updated);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var record = await _service.GetByIdAsync(id);
            if (record == null)
                return NotFound(new { Message = "Record not found" });

            return Ok(record);
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] int? contractId,
            [FromQuery] string? type,
            [FromQuery] string? result,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
        {
            var list = await _service.ListAsync(contractId, type, result, fromDate, toDate);
            return Ok(list);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound(new { Message = "Record not found" });

            return Ok(new { Message = "Compliance record deleted" });
        }
    }
}