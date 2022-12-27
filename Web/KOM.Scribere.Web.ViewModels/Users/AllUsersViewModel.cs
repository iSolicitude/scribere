using System.Collections.Generic;

namespace KOM.Scribere.Web.ViewModels.Users;

public class AllUsersViewModel
{
    public ICollection<UserViewModel> ApplicationUsers = new HashSet<UserViewModel>();
}