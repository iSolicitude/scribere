using Microsoft.AspNetCore.Mvc;

namespace KOM.Scribere.Web.Views.Shared.Components.RightSidebar;

public class RightSidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View();
    }
}
