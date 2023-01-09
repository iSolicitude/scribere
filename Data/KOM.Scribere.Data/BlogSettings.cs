namespace KOM.Scribere.Data;

using KOM.Scribere.Data.Models;

public class BlogSettings
{
    public string BlogName { get; set; } = null!;

    public int CommentsCloseAfterDays { get; set; } = 10;

    public bool DisplayComments { get; set; } = true;

    public bool NotifyOnNewComments { get; set; } = true;

    public string NotifyOnNewCommentsSubject { get; set; } = "New comment";

    public PostListView ListView { get; set; } = PostListView.TitlesAndExcerpts;

    public string Owner { get; set; } = "The Owner";

    public int PostsPerPage { get; set; } = 4;
}
