namespace KOM.Scribere.Data.Models;

using System.ComponentModel.DataAnnotations;

using KOM.Scribere.Data.Common.Models;

public class FavoritePost : BaseDeletableModel<string>
{
    [Required]
    public string UserId { get; set; }

    public virtual User User { get; set; }

    public virtual Post Post { get; set; }
}
