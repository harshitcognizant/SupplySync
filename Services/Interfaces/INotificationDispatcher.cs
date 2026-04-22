using SupplySync.Constants.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplySync.Services.Interfaces
{
    public interface INotificationDispatcher
    {
        /// <summary>
        /// Send a notification for the given event. Payload provides values used by the template placeholders.
        /// If the template uses role-based targets the dispatcher will resolve users by role.
        /// To send to specific users, provide overrideUserIds (ToUsers).
        /// </summary>
        Task SendNotificationAsync(NotificationEvent eventKey, IDictionary<string, object> payload, IEnumerable<int>? overrideUserIds = null);
    }
}