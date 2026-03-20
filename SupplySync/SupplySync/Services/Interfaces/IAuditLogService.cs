// /SupplySync/Services/Interfaces/IAuditLogService.cs
using SupplySync.DTOs.Audit;
using SupplySync.DTOs.AuditLog;
using SupplySync.DTOs.AuditLogs;

namespace SupplySync.Services.Interfaces
{
	public interface IAuditLogService
	{
		Task<AuditLogListResponseDto> GetAuditLogsAsync(AuditLogFiltersDto filters);

		// Reusable writer you can call from any service after success
		Task WriteAsync(int? userId, string? userName, string action, string resource);

		// Optional convenience overloads (if you later want to infer from ClaimsPrincipal):
		// Task WriteAsync(ClaimsPrincipal? user, string action, string resource);
	}
}