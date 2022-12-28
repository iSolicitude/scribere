namespace KOM.Scribere.Web.ViewModels.Profile;

using System;
using System.Collections.Generic;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class DownloadPersonalDataViewModel : IMapFrom<User>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    // Audit info
    public DateTime CreatedOn { get; set; }

    public string Town { get; set; }

    public string PostalCode { get; set; }

    public string CountryName { get; set; }

    public string Address { get; set; }

    public ICollection<PersonalDataCommentsViewModel> Comments { get; set; }

    public ICollection<PersonalDataFavoriteFroductsViewModel> FavoriteProducts { get; set; }
}
