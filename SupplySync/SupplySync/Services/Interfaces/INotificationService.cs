// /SupplySync/Services/Interfaces/INotificationService.cs
using SupplySync.DTOs.Notification;

namespace SupplySync.Services.Interfaces
{
	public interface INotificationService
	{
		// Admin
		Task<NotificationResponseDto> GetAdminAsync(int notificationId);
		Task<NotificationListResponseDto> ListAdminAsync(NotificationFiltersDto filters);
		Task<List<NotificationResponseDto>> SendAsync(CreateBulkNotificationRequestDto dto); // send to userIds / roleTypes

		// Self (user)
		Task<NotificationListResponseDto> ListMyAsync(int pathUserId, int callerUserId, NotificationFiltersDto filters);
		Task<NotificationResponseDto> UpdateMyAsync(int notificationId, int callerUserId, UpdateNotificationRequestDto dto, bool isAdmin);
		Task<int> MarkAllAsReadAsync(int pathUserId, int callerUserId, bool isAdmin);
		Task<NotificationResponseDto> CreateAsync(CreateNotificationRequestDto dto);
	}
}