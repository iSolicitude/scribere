namespace KOM.Scribere.Web.Views.Shared.Components.Navbar;

using Microsoft.AspNetCore.Mvc;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View();
    }
}
