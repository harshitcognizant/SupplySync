using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Finance;
using SupplySync.Services.Interfaces;
namespace SupplySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Authorize(Roles = "FinanceOfficer")]        // Finance creates payment
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto dto)
        {
            var id = await _paymentService.CreatePaymentAsync(dto);
            return Ok(new { Message = "Payment recorded", PaymentId = id });
        }

        [HttpPut("{paymentId}")]
        [Authorize(Roles = "FinanceOfficer")]       // Finance updates payment
        public async Task<IActionResult> UpdatePayment(int paymentId, [FromBody] UpdatePaymentRequestDto dto)
        {
            await _paymentService.UpdatePaymentAsync(paymentId, dto);
            return Ok(new { Message = "Payment updated successfully" , PaymentId = paymentId });
        }

        [HttpGet("{paymentId}")]
        [Authorize(Roles = "VendorUser,FinanceOfficer,ProcurementOfficer,ComplianceOfficer,Admin")]  // Read access for multiple roles
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
                return NotFound(new { Message = $"Payment with ID {paymentId} not found." });
            return Ok(payment);
        }

        [HttpGet]
        [Authorize(Roles = "FinanceOfficer,ComplianceOfficer,Admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }
    }
}
