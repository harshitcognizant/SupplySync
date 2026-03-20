using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Notification
{
	public class UpdateNotificationRequestDto
	{
		// For user self update we use only these:
		public NotificationStatus? Status { get; set; } // Read / Archived
		public bool? IsDeleted { get; set; }

		// NOTE: admin update can still use your existing Update route if needed.
	}

}
