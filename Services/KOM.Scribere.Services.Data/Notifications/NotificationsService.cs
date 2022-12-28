using System.Linq;
using KOM.Scribere.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;
using KOM.Scribere.Web.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace KOM.Scribere.Services.Data.Notifications;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationsService : INotificationsService
{
    private readonly IDeletableEntityRepository<User> usersRepository;

    private readonly IDeletableEntityRepository<UserNotification> notificationsRepository;

    public NotificationsService(
        IDeletableEntityRepository<User> usersRepository,
        IDeletableEntityRepository<UserNotification> notificationsRepository)
    {
        this.usersRepository = usersRepository;
        this.notificationsRepository = notificationsRepository;
    }

    public async Task<string> AddMessageNotificationAsync(string senderUsername, string receiverUsername, string message, string groupId)
    {
        var senderId = await this.usersRepository
            .AllAsNoTracking()
            .Where(u => u.UserName == senderUsername)
            .Select(u => u.Id)
            .SingleOrDefaultAsync();

        var notification = new UserNotification
        {
            ReceiverUsername = receiverUsername,
            Text = message,
            SenderId = senderId,
            Link = $"/chat/with/{senderUsername}/group/{groupId}",
        };

        // Delete notifications when more than 50
        var notifications = await this.notificationsRepository
            .All()
            .Where(n => n.ReceiverUsername == receiverUsername)
            .OrderBy(n => n.CreatedOn)
            .ToListAsync();

        if (notifications.Count + 1 > GlobalConstants.MaxChatNotificationsPerUser)
        {
            notifications = notifications
                .Take(GlobalConstants.MaxChatNotificationsPerUser - 1)
                .ToList();

            this.notificationsRepository.DeleteRange(notifications);
        }

        await this.notificationsRepository.AddAsync(notification);
        await this.notificationsRepository.SaveChangesAsync();

        return notification.Id;
    }

    public async Task<IEnumerable<NotificationViewModel>> GetUserNotificationsAsync(string currentUsername)
    {
        var userNotifications = await this.notificationsRepository
            .AllAsNoTracking()
            .Where(n => n.ReceiverUsername == currentUsername)
            .To<NotificationViewModel>()
            .OrderByDescending(n => n.CreatedOn)
            .ToListAsync();

        return userNotifications;
    }

    public async Task<NotificationViewModel> GetNotificationByIdAsync(string id)
    {
        var notification = await this.notificationsRepository
            .AllAsNoTracking()
            .Where(n => n.Id == id)
            .To<NotificationViewModel>()
            .SingleOrDefaultAsync();

        return notification;
    }

    public async Task<int> GetUserNotificationsCountAsync(string receiverUsername)
    {
        var count = await this.notificationsRepository
            .AllAsNoTracking()
            .CountAsync(n => n.ReceiverUsername == receiverUsername);

        return count;
    }

    public async Task<bool> DeleteNotificationAsync(string id)
    {
        var affectedRows = await this.notificationsRepository
            .All()
            .Where(n => n.Id == id)
            .UpdateAsync(n => new UserNotification
            {
                IsDeleted = true,
                DeletedOn = DateTime.UtcNow,
            });

        return affectedRows == 1;
    }

    public async Task<bool> DeleteAllNotificationsByUserIdAsync(string currentUserId, string currentUsername)
    {
        var affectedRows = await this.notificationsRepository
            .All()
            .Where(n => n.SenderId == currentUserId || n.ReceiverUsername == currentUsername)
            .UpdateAsync(n => new UserNotification
            {
                IsDeleted = true,
                DeletedOn = DateTime.UtcNow,
            });

        return affectedRows >= 0;
    }
}