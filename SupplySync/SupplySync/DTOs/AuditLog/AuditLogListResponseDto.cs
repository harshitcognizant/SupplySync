using System.Collections.Generic;

namespace SupplySync.DTOs.AuditLogs
{
	public class AuditLogListResponseDto
	{
		public List<AuditLogResponseDto> Items { get; set; } = new();

		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
	}
}