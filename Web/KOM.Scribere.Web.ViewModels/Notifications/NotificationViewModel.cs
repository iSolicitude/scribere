namespace KOM.Scribere.Web.ViewModels.Notifications;

using System;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class NotificationViewModel : IMapFrom<UserNotification>
{
    public string Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public string SenderId { get; set; }

    public string SenderUsername { get; set; }

    public string Link { get; set; }

    public string Text { get; set; }
}
