// /SupplySync/Repositories/Interfaces/INotificationRepository.cs
using SupplySync.DTOs.Notification;
using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
	public interface INotificationRepository
	{
		Task<Notification> InsertAsync(Notification notification);
		Task InsertRangeAsync(IEnumerable<Notification> notifications);
		Task<Notification?> GetByIdWithNavAsync(int id);
		Task<Notification> UpdateAsync(Notification notification);
		Task<(List<Notification> Items, int TotalCount)> GetPagedAsync(NotificationFiltersDto filters);
		Task<int> MarkAllAsReadAsync(int userId); // bulk update
	}
}