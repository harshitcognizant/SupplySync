namespace SupplySync.DTOs.AuditLog
{
	public class AuditLogFiltersDto
	{
		public int? UserID { get; set; }
		public string? Action { get; set; }      // exact match (e.g., "Login", "UserCreated")
		public string? Resource { get; set; }    // contains match (e.g., "User:42")

		public DateTime? FromUtc { get; set; }   // inclusive
		public DateTime? ToUtc { get; set; }     // inclusive

		public string? Search { get; set; }      // free text on Action/Resource/User.Name

		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public bool Desc { get; set; } = true;   // order by Timestamp desc by default
	}

}
