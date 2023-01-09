namespace KOM.Scribere.Web.Views.Shared.Components.Sidebar;

using System.Linq;

using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;
using KOM.Scribere.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class SidebarViewComponent : ViewComponent
{
    private readonly IDeletableEntityRepository<Post> postsRepository;

    public SidebarViewComponent(
        IDeletableEntityRepository<Post> postsRepository)
    {
        this.postsRepository = postsRepository;
    }

    public IViewComponentResult Invoke()
    {
        var model = new SidebarViewModel
        {
            RecentPosts =
                this.postsRepository.All().OrderByDescending(x => x.CreatedOn)
                    .To<RecentBlogPostViewModel>().Take(5).ToList(),
        };

        return this.View(model);
    }
}