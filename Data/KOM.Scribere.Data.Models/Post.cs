using System.ComponentModel.DataAnnotations.Schema;

namespace KOM.Scribere.Data.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using KOM.Scribere.Data.Common.Models;

public class Post : BaseDeletableModel<string>
{
    public Post()
    {
        this.Id = Guid.NewGuid().ToString();
        this.Comments = new HashSet<Comment>();
        this.Tags = new List<string>();
        this.Categories = new List<string>();
        this.PublishDate = DateTimeOffset.UtcNow;
    }

    [NotMapped]
    public IList<string> Categories { get; set; }

    [NotMapped]
    public IList<string> Tags { get; set; }

    public ICollection<Comment> Comments { get; }

    public PostType Type { get; set; }

    [Required]
    [DataType(DataType.Html)]
    [Display(Name = "Content")]
    [MinLength(10, ErrorMessage = "The {0} must be at least {1} characters long.")]
    public string Content { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Url)]
    [Display(Name = "Cover Photo")]
    public string ImageUrl { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "Video")]
    public string VideoUrl { get; set; }

    [Display(Name = "Image/Video url")]
    [DataType(DataType.Url)]
    public string Url { get; set; }

    [Required]
    [DataType(DataType.Html)]
    [Display(Name = "Description/Short Content")]
    [MinLength(10, ErrorMessage = "The {0} must be at least {1} characters long.")]
    public string Excerpt { get; set; } = string.Empty;

    public bool IsPublished { get; set; } = true;

    public DateTimeOffset PublishDate { get; set; }

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Slug { get; set; } = string.Empty;

    public bool IsAchived { get; set; } = false;

    public bool IsPinned { get; set; }

    [Required]
    [Display(Name = "Title")]
    [MinLength(3, ErrorMessage = "The {0} must be at least {1} characters long.")]
    public string Title { get; set; } = string.Empty;

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "The slug should be lower case.")]
    public static string CreateSlug(string title)
    {
        title = title?.ToLowerInvariant().Replace(
            " ", "-", StringComparison.OrdinalIgnoreCase) ?? string.Empty;
        title = RemoveDiacritics(title);
        title = RemoveReservedUrlCharacters(title);

        return title.ToLowerInvariant();
    }

    public bool AreCommentsOpen(int commentsCloseAfterDays) =>
        this.PublishDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;

    public string GetEncodedLink() => $"/blog/{System.Net.WebUtility.UrlEncode(this.Slug)}/";

    public string GetLink() => $"/blog/{this.Slug}/";

    public bool IsVisible() => this.PublishDate <= DateTime.UtcNow && this.IsPublished;

    public string RenderContent()
    {
        var result = this.Content;

        // Set up lazy loading of images/iframes
        if (!string.IsNullOrEmpty(result))
        {
            // Set up lazy loading of images/iframes
            var replacement = " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
            var pattern = "(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)";
            result = Regex.Replace(result, pattern, m => m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value);

            // Youtube content embedded using this syntax: [youtube:xyzAbc123]
            var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
            result = Regex.Replace(
                result,
                @"\[youtube:(.*?)\]",
                m => string.Format(CultureInfo.InvariantCulture, video, m.Groups[1].Value));
        }

        return result;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string RemoveReservedUrlCharacters(string text)
    {
        var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

        foreach (var chr in reservedCharacters)
        {
            text = text.Replace(chr, string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return text;
    }
}
