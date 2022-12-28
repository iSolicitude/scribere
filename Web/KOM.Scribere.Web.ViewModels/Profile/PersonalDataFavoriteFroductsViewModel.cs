namespace KOM.Scribere.Web.ViewModels.Profile;

using System;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class PersonalDataFavoriteFroductsViewModel : IMapFrom<FavoritePost>
{
    public int Id { get; set; }

    public string PostName { get; set; }

    public DateTime CreatedOn { get; set; }
}