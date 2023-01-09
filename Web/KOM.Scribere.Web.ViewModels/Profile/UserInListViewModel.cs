namespace KOM.Scribere.Web.ViewModels.Profile;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class UserInListViewModel : IMapFrom<User>
{
    public string Id { get; set; }

    public string UserName { get; set; }
}
