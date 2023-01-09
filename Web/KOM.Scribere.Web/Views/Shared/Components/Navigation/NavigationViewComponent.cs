using Microsoft.AspNetCore.Mvc;

namespace KOM.Scribere.Web.Views.Shared.Components.Navigation;

public class NavigationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View();
    }
}
