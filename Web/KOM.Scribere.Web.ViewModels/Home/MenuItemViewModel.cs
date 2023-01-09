namespace KOM.Scribere.Web.ViewModels.Home;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class MenuItemViewModel : IMapFrom<Page>
{
    public string Title { get; set; }

    public string Permalink { get; set; }
}
