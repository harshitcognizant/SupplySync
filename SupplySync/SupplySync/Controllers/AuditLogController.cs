// /SupplySync/Controllers/AuditController.cs
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.AuditLog;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuditLogController : ControllerBase
	{
		private readonly IAuditLogService _auditService;
		public AuditLogController(IAuditLogService auditService) => _auditService = auditService;

		// GET /audit?userId=&action=&resource=&fromUtc=&toUtc=&search=&pageNumber=1&pageSize=20&desc=true
		[HttpGet("audit")]
		// [Authorize(Roles = "Admin,ComplianceOfficer")] // uncomment to enforce RBAC
		public async Task<IActionResult> Get([FromQuery] AuditLogFiltersDto filters)
		{
			var result = await _auditService.GetAuditLogsAsync(filters);
			return Ok(result);
		}
	}
}