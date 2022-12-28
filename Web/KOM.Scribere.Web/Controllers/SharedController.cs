namespace KOM.Scribere.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

public class SharedController : BaseController
{
    public IActionResult Error() => this.View(this.Response.StatusCode);

    /// <summary>
    /// This is for use in wwwroot/serviceworker.js to support offline scenarios
    /// </summary>
    public IActionResult Offline() => this.View();
}