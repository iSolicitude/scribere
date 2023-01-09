namespace KOM.Scribere.Web.ViewModels;

using System;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class RecentBlogPostViewModel : IMapFrom<Post>
{
    public string Id { get; set; }

    public string Title { get; set; }

    public PostType Type { get; set; }

    public DateTime CreatedOn { get; set; }

    public string GetLink { get; set; }

    public string IconClass => this.Type == PostType.Video ? "fab fa-youtube-square" : "fa fa-file-alt";
}
