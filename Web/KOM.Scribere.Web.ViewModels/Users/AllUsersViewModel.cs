namespace KOM.Scribere.Web.ViewModels.Users;

using System.Collections.Generic;

public class AllUsersViewModel
{
    public ICollection<UserViewModel> ApplicationUsers = new HashSet<UserViewModel>();
}
