namespace KOM.Scribere.Web.ViewModels.Blog;

using System;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;
using Microsoft.AspNetCore.Http;

public class PostViewModel : IMapFrom<Post>, IMapTo<Post>
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public IFormFile Image { get; set; }

    public IFormFile Video { get; set; }

    public string ImageUrl { get; set; }

    public string VideoUrl { get; set; }

    public string MetaDescription { get; set; }

    public string MetaKeywords { get; set; }

    public DateTime CreatedOn { get; set; }
}
