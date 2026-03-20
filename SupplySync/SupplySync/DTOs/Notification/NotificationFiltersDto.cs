using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Notification
{
	public class NotificationFiltersDto
	{
		public int? UserID { get; set; }
		public int? ContractID { get; set; }
		public NotificationCategory? Category { get; set; }
		public NotificationStatus? Status { get; set; }

		public DateTime? FromUtc { get; set; } // inclusive (uses CreatedDate)
		public DateTime? ToUtc { get; set; }   // inclusive (uses CreatedDate)

		public string? Search { get; set; }    // on Message and User.Name

		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public bool Desc { get; set; } = true; // order by CreatedDate
	}

}
