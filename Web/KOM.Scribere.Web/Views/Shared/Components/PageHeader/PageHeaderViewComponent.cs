namespace KOM.Scribere.Web.Views.Shared.Components.PageHeader;

using Microsoft.AspNetCore.Mvc;

public class PageHeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View();
    }
}
