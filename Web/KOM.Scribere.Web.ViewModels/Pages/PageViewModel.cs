namespace KOM.Scribere.Web.ViewModels.Pages;

using System;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class PageViewModel : IMapFrom<Page>
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime LastModified { get; set; }
}
