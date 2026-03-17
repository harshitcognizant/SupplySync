using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Audit;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _service;

        public AuditController(IAuditService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditRequestDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { Message = "Audit created", AuditID = id });
        }

        [HttpPut("{auditId}")]
        public async Task<IActionResult> Update(int auditId, [FromBody] UpdateAuditRequestDto dto)
        {
            var updated = await _service.UpdateAsync(auditId, dto);
            if (updated == null)
                return NotFound(new { Message = "Audit not found" });

            return Ok(updated);
        }

        [HttpGet("{auditId}")]
        public async Task<IActionResult> Get(int auditId)
        {
            var record = await _service.GetByIdAsync(auditId);
            if (record == null)
                return NotFound(new { Message = "Audit not found" });

            return Ok(record);
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] int? complianceOfficerId,
            [FromQuery] string? status,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
        {
            var list = await _service.ListAsync(complianceOfficerId, status, fromDate, toDate);
            return Ok(list);
        }

        [HttpDelete("{auditId}")]
        public async Task<IActionResult> Delete(int auditId)
        {
            var ok = await _service.DeleteAsync(auditId);
            if (!ok) return NotFound(new { Message = "Audit not found" });

            return Ok(new { Message = "Audit deleted" });
        }
    }
}