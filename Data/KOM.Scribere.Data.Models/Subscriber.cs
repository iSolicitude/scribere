namespace KOM.Scribere.Data.Models;

using KOM.Scribere.Data.Common.Models;

public class Subscriber : BaseDeletableModel<int>
{
    public string Email { get; set; }
}
