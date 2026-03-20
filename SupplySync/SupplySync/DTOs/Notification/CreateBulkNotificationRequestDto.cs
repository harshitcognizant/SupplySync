using System.ComponentModel.DataAnnotations;
using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Notification
{
	public class CreateBulkNotificationRequestDto
	{
		public List<int>? UserIDs { get; set; }            // optional
		public List<RoleType>? RoleTypes { get; set; }     // optional

		public int? ContractID { get; set; }

		[Required]
		public string Message { get; set; } = default!;

		[Required]
		public NotificationCategory Category { get; set; }

		public NotificationStatus? Status { get; set; } // defaults to Unread if null
	}

}
