using System;

namespace SupplySync.DTOs.AuditLogs
{
	public class AuditLogResponseDto
	{
		public int AuditID { get; set; }

		public int? UserID { get; set; }
		public string? UserName { get; set; } // from navigation (nullable for system jobs)

		public string Action { get; set; } = default!;
		public string Resource { get; set; } = default!;

		public DateTime Timestamp { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}