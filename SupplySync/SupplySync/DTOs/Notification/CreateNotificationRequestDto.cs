using System.ComponentModel.DataAnnotations;
using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Notification
{
	public class CreateNotificationRequestDto
	{
		[Required]
		public int UserID { get; set; }

		public int? ContractID { get; set; }

		[Required]
		public string Message { get; set; } = default!;

		[Required]
		public NotificationCategory Category { get; set; }

		// Optional: if omitted, service will default to Unread
		public NotificationStatus? Status { get; set; }
	}

}
