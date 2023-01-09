namespace KOM.Scribere.Web.ViewModels.Users;

using System;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class UserViewModel : IMapFrom<User>
{
    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName => $"{this.FirstName} {this.LastName}";

    public string Role { get; set; }

    public DateTime CreatedOn { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public string Address1 { get; set; }

    public string Address2 { get; set; }
}
