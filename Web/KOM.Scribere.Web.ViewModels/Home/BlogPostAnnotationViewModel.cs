namespace KOM.Scribere.Web.ViewModels.Home;

using System;

using AutoMapper;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class BlogPostAnnotationViewModel : IMapFrom<Post>, IHaveCustomMappings
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ImageOrVideoUrl { get; set; }

    public void CreateMappings(IProfileExpression configuration)
    {
        configuration.CreateMap<Post, BlogPostAnnotationViewModel>().ForMember(
            m => m.Content,
            opt => opt.MapFrom(u => u.Excerpt));
    }
}
