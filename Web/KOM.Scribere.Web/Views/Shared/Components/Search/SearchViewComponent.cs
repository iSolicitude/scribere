using Microsoft.AspNetCore.Mvc;

namespace KOM.Scribere.Web.Views.Shared.Components.Search;

public class SearchViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View();
    }
}
