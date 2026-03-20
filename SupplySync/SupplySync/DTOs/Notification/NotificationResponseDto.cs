using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Notification
{
	public class NotificationResponseDto
	{
        public int NotificationID { get; set; }

		public int UserID { get; set; }
		public string? UserName { get; set; }
			
		public int? ContractID { get; set; }

		public string Message { get; set; } = default!;
		public NotificationCategory Category { get; set; }
		public NotificationStatus Status { get; set; }
	
		public bool IsDeleted { get; set; }
	
		public DateTime CreatedDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
    }

}
