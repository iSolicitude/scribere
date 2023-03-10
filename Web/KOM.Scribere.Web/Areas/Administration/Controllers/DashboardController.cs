namespace KOM.Scribere.Web.Areas.Administration.Controllers;

using KOM.Scribere.Services.Data;
using KOM.Scribere.Web.ViewModels.Administration.Dashboard;

using Microsoft.AspNetCore.Mvc;

public class DashboardController : AdministrationController<DashboardController>
{
    private readonly ISettingsService settingsService;

    public DashboardController(ISettingsService settingsService)
    {
        this.settingsService = settingsService;
    }

    public IActionResult Index()
    {
        var viewModel = new IndexViewModel { SettingsCount = this.settingsService.GetCount(), };
        return this.View(viewModel);
    }
}