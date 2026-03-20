using SupplySync.DTOs.AuditLog;
using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
	public interface IAuditLogRepository
	{
		Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(AuditLogFiltersDto filters);
		Task<AuditLog> InsertAsync(AuditLog log);
	}

}
