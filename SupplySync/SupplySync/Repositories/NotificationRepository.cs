// /SupplySync/Repositories/NotificationRepository.cs
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.DTOs.Notification;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Constants.Enums;

namespace SupplySync.Repositories
{
	public class NotificationRepository : INotificationRepository
	{
		private readonly AppDbContext _context;
		public NotificationRepository(AppDbContext context) => _context = context;

		public async Task<Notification> InsertAsync(Notification notification)
		{
			await _context.Notifications.AddAsync(notification);
			await _context.SaveChangesAsync();
			return notification;
		}

		public async Task InsertRangeAsync(IEnumerable<Notification> notifications)
		{
			await _context.Notifications.AddRangeAsync(notifications);
			await _context.SaveChangesAsync();
		}

		public async Task<Notification?> GetByIdWithNavAsync(int id)
		{
			return await _context.Notifications
				.Include(n => n.User)
				.Include(n => n.Contract)
				.FirstOrDefaultAsync(n => n.NotificationID == id);
		}

		public async Task<Notification> UpdateAsync(Notification notification)
		{
			_context.Notifications.Update(notification);
			await _context.SaveChangesAsync();
			return notification;
		}

		public async Task<(List<Notification> Items, int TotalCount)> GetPagedAsync(NotificationFiltersDto filters)
		{
			var q = _context.Notifications
				.Include(n => n.User)
				.Include(n => n.Contract)
				.Where(n => !n.IsDeleted)
				.AsQueryable();

			if (filters.UserID.HasValue) q = q.Where(n => n.UserID == filters.UserID.Value);
			if (filters.ContractID.HasValue) q = q.Where(n => n.ContractID == filters.ContractID.Value);
			if (filters.Category.HasValue) q = q.Where(n => n.Category == filters.Category.Value);
			if (filters.Status.HasValue) q = q.Where(n => n.Status == filters.Status.Value);
			if (filters.FromUtc.HasValue) q = q.Where(n => n.CreatedDate >= filters.FromUtc.Value);
			if (filters.ToUtc.HasValue) q = q.Where(n => n.CreatedDate <= filters.ToUtc.Value);
			if (!string.IsNullOrWhiteSpace(filters.Search))
			{
				var s = filters.Search.Trim();
				q = q.Where(n =>
					(n.Message != null && n.Message.Contains(s)) ||
					(n.User != null && n.User.Name != null && n.User.Name.Contains(s))
				);
			}

			q = filters.Desc
				? q.OrderByDescending(n => n.CreatedDate).ThenByDescending(n => n.NotificationID)
				: q.OrderBy(n => n.CreatedDate).ThenBy(n => n.NotificationID);

			var total = await q.CountAsync();

			var page = filters.PageNumber < 1 ? 1 : filters.PageNumber;
			var size = filters.PageSize < 1 ? 20 : filters.PageSize;

			var items = await q.Skip((page - 1) * size)
							   .Take(size)
							   .ToListAsync();

			return (items, total);
		}

		public async Task<int> MarkAllAsReadAsync(int userId)
		{
			// Update unread to read for the user
			var rows = await _context.Notifications
				.Where(n => n.UserID == userId && !n.IsDeleted && n.Status == NotificationStatus.Unread)
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(n => n.Status, NotificationStatus.Read)
					.SetProperty(n => n.UpdatedAt, DateTime.UtcNow));

			return rows;
		}
	}
}