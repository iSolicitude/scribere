namespace KOM.Scribere.Web.Views.Shared.Components.Menu;

using System.Linq;

using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;
using KOM.Scribere.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

public class MenuViewComponent : ViewComponent
{
    private readonly IDeletableEntityRepository<Page> pagesRepository;

    public MenuViewComponent(IDeletableEntityRepository<Page> pagesRepository)
    {
        this.pagesRepository = pagesRepository;
    }

    public IViewComponentResult Invoke()
    {
        var menuItems = this.pagesRepository
            .All()
            .Where(p => !p.IsDeleted)
            .To<MenuItemViewModel>()
            .ToList();

        return this.View(menuItems);
    }
}