namespace KOM.Scribere.Web.Controllers;

using System.Security.Claims;
using System.Threading.Tasks;

using KOM.Scribere.Services.Data.Notifications;
using KOM.Scribere.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class NotificationsController : BaseController
{
    private readonly INotificationsService notificationsService;

    private readonly IHubContext<NotificationHub> hubContext;

    public NotificationsController(
        INotificationsService notificationsService,
        IHubContext<NotificationHub> hubContext)
    {
        this.notificationsService = notificationsService;
        this.hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        var username = this.User.FindFirstValue(ClaimTypes.Name);

        var viewModel = await this.notificationsService.GetUserNotificationsAsync(username);

        return this.View(viewModel);
    }

    [HttpPost]
    public async Task<bool> Delete(string id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = this.User.FindFirstValue(ClaimTypes.Name);

        var isDeleted = await this.notificationsService.DeleteNotificationAsync(id);
        await this.ChangeNotificationCounterAsync(isDeleted, username, userId);

        return isDeleted;
    }

    private async Task ChangeNotificationCounterAsync(bool isForChange, string username, string userId)
    {
        if (isForChange)
        {
            int count = await this.notificationsService.GetUserNotificationsCountAsync(username);

            await this.hubContext.Clients.User(userId).SendAsync("ReceiveNotification", count, false);
        }
    }
}