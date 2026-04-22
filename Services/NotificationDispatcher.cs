using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Notification;
using SupplySync.Notifications;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public NotificationDispatcher(INotificationService notificationService, IUserRepository userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task SendNotificationAsync(NotificationEvent eventKey, IDictionary<string, object> payload, IEnumerable<int>? overrideUserIds = null)
        {
            if (!NotificationTemplates.Registry.TryGetValue(eventKey, out var template))
                throw new InvalidOperationException($"Notification template for event '{eventKey}' not found.");

            // Build message
            var message = template.MessageFormatter(payload ?? new Dictionary<string, object>());

            // If caller provided explicit user IDs, use those
            if (overrideUserIds != null && overrideUserIds.Any())
            {
                var dto = new CreateBulkNotificationRequestDto
                {
                    UserIDs = overrideUserIds.ToList(),
                    Message = message,
                    Category = template.Category,
                    Status = null
                };

                await _notificationService.SendAsync(dto);
                return;
            }

            // If template targets roles, call notification service with RoleTypes
            if (template.UseRoleTargets && template.ToRoles != null && template.ToRoles.Count > 0)
            {
                var dto = new CreateBulkNotificationRequestDto
                {
                    RoleTypes = template.ToRoles.ToList(),
                    Message = message,
                    Category = template.Category,
                    Status = null
                };

                await _notificationService.SendAsync(dto);
                return;
            }

            // Otherwise nothing to send (template expects explicit users)
            // Optionally log or throw; we'll throw to make missing target explicit
            throw new InvalidOperationException($"Notification template for event '{eventKey}' does not specify roles and no overrideUserIds were provided.");
        }
    }
}