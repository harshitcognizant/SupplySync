// /SupplySync/Services/NotificationService.cs
using AutoMapper;
using SupplySync.DTOs.Notification;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using SupplySync.Constants.Enums;

namespace SupplySync.Services
{
	public class NotificationService : INotificationService
	{
		private readonly INotificationRepository _notificationRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;

		public NotificationService(
			INotificationRepository notificationRepository,
			IUserRepository userRepository,
			IMapper mapper)
		{
			_notificationRepository = notificationRepository;
			_userRepository = userRepository;
			_mapper = mapper;
		}

		// ===== Admin =====
		public async Task<NotificationResponseDto> GetAdminAsync(int notificationId)
		{
			var entity = await _notificationRepository.GetByIdWithNavAsync(notificationId)
				?? throw new KeyNotFoundException("Notification not found.");
			return _mapper.Map<NotificationResponseDto>(entity);
		}

		public async Task<NotificationListResponseDto> ListAdminAsync(NotificationFiltersDto filters)
		{
			var (items, total) = await _notificationRepository.GetPagedAsync(filters);
			return new NotificationListResponseDto
			{
				Items = _mapper.Map<List<NotificationResponseDto>>(items),
				PageNumber = filters.PageNumber,
				PageSize = filters.PageSize,
				TotalCount = total
			};
		}

		// Accept multiple userIds and/or roleTypes
		public async Task<List<NotificationResponseDto>> SendAsync(CreateBulkNotificationRequestDto dto)
		{
			var userIds = new HashSet<int>();

			if (dto.UserIDs != null)
			{
				foreach (var id in dto.UserIDs.Where(id => id > 0))
					userIds.Add(id);
			}

			if (dto.RoleTypes != null && dto.RoleTypes.Count > 0)
			{
				var fromRoles = await _userRepository.GetActiveUserIdsByRoleTypesAsync(dto.RoleTypes);
				foreach (var id in fromRoles)
					userIds.Add(id);
			}

			if (userIds.Count == 0)
				throw new InvalidOperationException("No target users resolved from UserIDs or RoleTypes.");

			var now = DateTime.UtcNow;
			var status = dto.Status ?? NotificationStatus.Unread;

			var notifications = userIds.Select(uid => new Notification
			{
				UserID = uid,
				ContractID = dto.ContractID,
				Message = dto.Message,
				Category = dto.Category,
				Status = status,
				IsDeleted = false,
				CreatedAt = now,
				UpdatedAt = now
				// CreatedDate is DB default
			}).ToList();

			await _notificationRepository.InsertRangeAsync(notifications);

			// Load with nav and map
			var responses = new List<NotificationResponseDto>(notifications.Count);
			foreach (var n in notifications)
			{
				var reloaded = await _notificationRepository.GetByIdWithNavAsync(n.NotificationID) ?? n;
				responses.Add(_mapper.Map<NotificationResponseDto>(reloaded));
			}
			return responses;
		}

		// ===== Self (user) =====

		// GET /notifications/my/{userId}
		public async Task<NotificationListResponseDto> ListMyAsync(int pathUserId, int callerUserId, NotificationFiltersDto filters)
		{
			filters.UserID = pathUserId; // enforce scope regardless of query

			var (items, total) = await _notificationRepository.GetPagedAsync(filters);
			return new NotificationListResponseDto
			{
				Items = _mapper.Map<List<NotificationResponseDto>>(items),
				PageNumber = filters.PageNumber,
				PageSize = filters.PageSize,
				TotalCount = total
			};
		}

		// PUT /notifications/my/{id} (mark read / archived / soft delete)
		public async Task<NotificationResponseDto> UpdateMyAsync(int notificationId, int callerUserId, UpdateNotificationRequestDto dto, bool isAdmin)
		{
			var entity = await _notificationRepository.GetByIdWithNavAsync(notificationId)
				?? throw new KeyNotFoundException("Notification not found.");

			if (!isAdmin && entity.UserID != callerUserId)
				throw new UnauthorizedAccessException("Cannot modify other users' notifications.");

			if (dto.Status.HasValue)
			{
				if (dto.Status is NotificationStatus.Read or NotificationStatus.Archived)
					entity.Status = dto.Status.Value;
				else
					throw new InvalidOperationException("Only Read or Archived are allowed in self update.");
			}

			if (dto.IsDeleted.HasValue)
				entity.IsDeleted = dto.IsDeleted.Value;

			entity.UpdatedAt = DateTime.UtcNow;

			var updated = await _notificationRepository.UpdateAsync(entity);
			var withNav = await _notificationRepository.GetByIdWithNavAsync(updated.NotificationID) ?? updated;
			return _mapper.Map<NotificationResponseDto>(withNav);
		}

		// PUT /notifications/my/{userId}/mark-all-read
		public async Task<int> MarkAllAsReadAsync(int pathUserId, int callerUserId, bool isAdmin)
		{
			if (!isAdmin && pathUserId != callerUserId)
				throw new UnauthorizedAccessException("Cannot modify other users' notifications.");

			var affected = await _notificationRepository.MarkAllAsReadAsync(pathUserId);
			return affected;
		}


		public async Task<NotificationResponseDto> CreateAsync(CreateNotificationRequestDto dto)
		{
			// Validate user exists and not deleted (reuse your existing validation)
			var user = await _userRepository.GetByIdAsync(dto.UserID)
				?? throw new KeyNotFoundException("User not found.");
			if (user.IsDeleted)
				throw new InvalidOperationException("Cannot create notifications for a deleted user.");

			var now = DateTime.UtcNow;

			var entity = new Notification
			{
				UserID = dto.UserID,
				ContractID = dto.ContractID,
				Message = dto.Message,
				Category = dto.Category,
				Status = dto.Status ?? NotificationStatus.Unread,
				IsDeleted = false,
				CreatedAt = now,
				UpdatedAt = now
				// CreatedDate is DB default (GETUTCDATE())
			};

			var created = await _notificationRepository.InsertAsync(entity);
			var withNav = await _notificationRepository.GetByIdWithNavAsync(created.NotificationID) ?? created;

			return _mapper.Map<NotificationResponseDto>(withNav);
		}

	}
}