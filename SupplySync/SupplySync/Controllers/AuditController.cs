using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Audit;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAudit([FromBody] CreateAuditDto dto)
        {
            var id = await _auditService.CreateAuditAsync(dto);
            return Ok(new { Message = "Audit created successfully", AuditID = id });
        }
    }

}
