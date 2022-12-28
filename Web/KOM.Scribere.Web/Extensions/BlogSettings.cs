namespace KOM.Scribere.Web.Extensions;

using KOM.Scribere.Data.Models;

public class BlogSettings
{
    public int CommentsCloseAfterDays { get; set; } = 10;

    public bool DisplayComments { get; set; } = true;

    public PostListView ListView { get; set; } = PostListView.TitlesAndExcerpts;

    public string Owner { get; set; } = "The Owner";

    public int PostsPerPage { get; set; } = 4;
}