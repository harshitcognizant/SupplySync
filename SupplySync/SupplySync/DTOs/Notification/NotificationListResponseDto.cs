namespace SupplySync.DTOs.Notification
{
	public class NotificationListResponseDto
	{
		public List<NotificationResponseDto> Items { get; set; } = new();

		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
	}

}
