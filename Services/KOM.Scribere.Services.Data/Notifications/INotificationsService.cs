namespace KOM.Scribere.Services.Data.Notifications;

using System.Collections.Generic;
using System.Threading.Tasks;

using KOM.Scribere.Web.ViewModels.Notifications;

public interface INotificationsService
{
    Task<string> AddMessageNotificationAsync(string senderUsername, string receiverUsername, string message, string groupId);

    Task<IEnumerable<NotificationViewModel>> GetUserNotificationsAsync(string currentUsername);

    Task<NotificationViewModel> GetNotificationByIdAsync(string id);

    Task<int> GetUserNotificationsCountAsync(string receiverUsername);

    Task<bool> DeleteNotificationAsync(string id);

    Task<bool> DeleteAllNotificationsByUserIdAsync(string currentUserId, string currentUsername);
}