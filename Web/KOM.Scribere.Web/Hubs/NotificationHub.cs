namespace KOM.Scribere.Web.Hubs;

using System.Linq;
using System.Threading.Tasks;

using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Data.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class NotificationHub : Hub
{
    private readonly INotificationsService notificationsService;

    private readonly IDeletableEntityRepository<User> usersRepository;

    public NotificationHub(
        INotificationsService notificationsService,
        IDeletableEntityRepository<User> usersRepository)
    {
        this.notificationsService = notificationsService;
        this.usersRepository = usersRepository;
    }

    public async Task GetUserNotificationsCount(bool playNotificationSound)
    {
        var username = this.Context.User.Identity.Name;

        var receiver = await this.usersRepository
            .AllAsNoTracking()
            .Where(u => u.UserName == username)
            .Select(u => new
            {
                u.Id,
                u.UserName,
            })
            .SingleOrDefaultAsync();

        var count = await this.notificationsService.GetUserNotificationsCountAsync(receiver.UserName);

        await this.Clients
            .User(receiver.Id)
            .SendAsync("ReceiveNotification", count, playNotificationSound);
    }
}