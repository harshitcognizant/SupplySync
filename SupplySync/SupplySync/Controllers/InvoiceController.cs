using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Finance;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        [Authorize(Roles = "VendorUser")]  // Vendor submits invoice
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequestDto dto)
        {
            var id = await _invoiceService.CreateInvoiceAsync(dto);
            return Ok(new{ Message = "Invoice submitted successfully", InvoiceID = id });
        }

        [HttpPut("{invoiceId}")]
        [Authorize(Roles = "FinanceOfficer,Admin")] // Finance approves/rejects invoice
        public async Task<IActionResult> UpdateInvoice(int invoiceId, [FromBody] UpdateInvoiceRequestDto dto)
        {
            
                await _invoiceService.UpdateInvoiceAsync(invoiceId, dto);
                return Ok(new
                {
                    Message = "Invoice updated successfully",
                    InvoiceID = invoiceId
                });
        }
        

        [HttpGet("{invoiceId}")]
        [Authorize(Roles = "VendorUser,FinanceOfficer,ProcurementOfficer,ComplianceOfficer,Admin")]
        public async Task<IActionResult> GetInvoice(int invoiceId)
        {
            var response = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            return response == null ? NotFound(new { Message = $"Invoice {invoiceId} not found." }) : Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "FinanceOfficer,ProcurementOfficer,ComplianceOfficer,Admin")]
        public async Task<IActionResult> ListInvoices()
        {
            var response = await _invoiceService.GetInvoiceListAsync();
            return Ok(response);
        }
    }
}
