namespace KOM.Scribere.Data.Models;

using System;
using System.ComponentModel.DataAnnotations;

using KOM.Scribere.Data.Common.Models;

public class UserNotification : BaseDeletableModel<string>
{
    public UserNotification()
    {
        this.Id = Guid.NewGuid().ToString();
    }

    [Required]
    public string ReceiverUsername { get; set; }

    [Required]
    public string Link { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public string SenderId { get; set; }

    public virtual User Sender { get; set; }
}