using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Report;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,ComplianceOfficer,ProcurementOfficer,FinanceOfficer,WarehouseManager")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ComplianceOfficer")]
        public async Task<IActionResult> Create([FromBody] CreateReportRequestDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { Message = "Report generated", ReportID = id });
        }

        [HttpPut("{reportId}")]
        [Authorize(Roles = "Admin,ComplianceOfficer")]
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
        [Authorize(Roles = "Admin,ComplianceOfficer")]
        public async Task<IActionResult> Delete(int reportId)
        {
            var ok = await _service.DeleteAsync(reportId);
            if (!ok) return NotFound(new { Message = "Report not found" });

            return Ok(new { Message = "Report deleted" });
        }

        [HttpGet("vendor-performance")]
        public async Task<IActionResult> VendorPerformance([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, [FromQuery] int top = 50)
                {
                    var data = await _service.VendorPerformanceAsync(fromUtc, toUtc, top);
                    return Ok(data);
                }

        [HttpGet("delivery-delays")]
        public async Task<IActionResult> DeliveryDelays([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, [FromQuery] int max = 100)
        {
            var data = await _service.DeliveryDelaysAsync(fromUtc, toUtc, max);
            return Ok(data);
        }

        [HttpGet("procurement-spending")]
        public async Task<IActionResult> ProcurementSpending([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
        {
            var data = await _service.TotalProcurementSpendingAsync(fromUtc, toUtc);
            return Ok(data);
        }

        [HttpGet("inventory-levels")]
        public async Task<IActionResult> InventoryLevels()
        {
            var data = await _service.InventoryLevelsAsync();
            return Ok(data);
        }

        [HttpGet("invoice-turnaround")]
        public async Task<IActionResult> InvoiceTurnaround([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
        {
            var data = await _service.InvoiceApprovalTurnaroundAsync(fromUtc, toUtc);
            return Ok(data);
        }
    }
}