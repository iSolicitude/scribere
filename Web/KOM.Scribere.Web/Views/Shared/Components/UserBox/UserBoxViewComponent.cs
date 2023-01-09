namespace KOM.Scribere.Web.Views.Shared.Components.UserBox;

using Microsoft.AspNetCore.Mvc;

public class UserBoxViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return this.View();
    }
}
