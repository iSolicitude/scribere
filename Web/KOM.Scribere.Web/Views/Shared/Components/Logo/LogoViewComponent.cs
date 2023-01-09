using Microsoft.AspNetCore.Mvc;

namespace KOM.Scribere.Web.Views.Shared.Components.Logo;

public class LogoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View();
    }
}
