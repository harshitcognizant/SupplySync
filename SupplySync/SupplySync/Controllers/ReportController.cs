using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Report;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportRequestDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { Message = "Report generated", ReportID = id });
        }

        [HttpPut("{reportId}")]
        public async Task<IActionResult> Update(int reportId, [FromBody] UpdateReportRequestDto dto)
        {
            var updated = await _service.UpdateAsync(reportId, dto);
            if (updated == null)
                return NotFound(new { Message = "Report not found" });

            return Ok(updated);
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> Get(int reportId)
        {
            var record = await _service.GetByIdAsync(reportId);
            if (record == null)
                return NotFound(new { Message = "Report not found" });

            return Ok(record);
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] string? scope,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var list = await _service.ListAsync(scope, fromDate, toDate);
            return Ok(list);
        }

        [HttpDelete("{reportId}")]
        public async Task<IActionResult> Delete(int reportId)
        {
            var ok = await _service.DeleteAsync(reportId);
            if (!ok) return NotFound(new { Message = "Report not found" });

            return Ok(new { Message = "Report deleted" });
        }
    }
}