using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KOM.Scribere.Web.Areas.Administration.Controllers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

using KOM.Scribere.Common;
using KOM.Scribere.Data;
using KOM.Scribere.Data.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services;
using KOM.Scribere.Services.Data;
using KOM.Scribere.Web.Controllers;
using KOM.Scribere.Web.Extensions;
using KOM.Scribere.Web.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebEssentials.AspNetCore.Pwa;

public class BlogController : AdministrationController<BlogController>
{
    private readonly IBlogService blog;

    private readonly WebManifest manifest;

    private readonly IOptionsSnapshot<BlogSettings> settings;
    
    private readonly IDeletableEntityRepository<Post> posts;

    private readonly ICloudinaryService cloudinaryService;
    


    public BlogController(
        IBlogService blog,
        IOptionsSnapshot<BlogSettings> settings,
        WebManifest manifest,
        IDeletableEntityRepository<Post> posts,
        ICloudinaryService cloudinaryService)
    {
        this.blog = blog;
        this.settings = settings;
        this.manifest = manifest;
        this.posts = posts;
        this.cloudinaryService = cloudinaryService;
    }

    public async Task<IActionResult> Index()
    {
        var post = await this.posts
            .All()
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync();

        if (this.posts == null)
        {
            return this.View("Error");
        }

        this._logger.LogInformation("Displaying list of blog posts");

        return this.View(post);
    }


    // GET: Administration/BlogPosts/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return this.NotFound();
        }

        var blogPost = await this.posts.All().FirstOrDefaultAsync(m => m.Id == id);
        if (blogPost == null)
        {
            return this.NotFound();
        }

        return this.View(blogPost);
    }

    [Route("/blog/category/{category}/{page:int?}")]
    [OutputCache(Profile = "default")]
    public async Task<IActionResult> Category(string category, int page = 0)
    {
        // get posts for the selected category.
        var posts = this.blog.GetPostsByCategory(category);

        // apply paging filter.
        var filteredPosts = posts.Skip(this.settings.Value.PostsPerPage * page).Take(this.settings.Value.PostsPerPage);

        // set the view option
        this.ViewData["ViewOption"] = this.settings.Value.ListView;

        this.ViewData[GlobalConstants.TotalPostCount] = await posts.CountAsync().ConfigureAwait(true);
        this.ViewData[GlobalConstants.Title] = $"{this.manifest.Name} {category}";
        this.ViewData[GlobalConstants.Description] = $"Articles posted in the {category} category";
        this.ViewData[GlobalConstants.prev] = $"/blog/category/{category}/{page + 1}/";
        this.ViewData[GlobalConstants.next] = $"/blog/category/{category}/{(page <= 1 ? null : page - 1 + "/")}";
        return this.View("~/Views/Home/Index.cshtml", filteredPosts.AsAsyncEnumerable());
    }

    [Route("/blog/tag/{tag}/{page:int?}")]
    [OutputCache(Profile = "default")]
    public async Task<IActionResult> Tag(string tag, int page = 0)
    {
        // get posts for the selected tag.
        var posts = this.blog.GetPostsByTag(tag);

        // apply paging filter.
        var filteredPosts = posts.Skip(this.settings.Value.PostsPerPage * page).Take(this.settings.Value.PostsPerPage);

        // set the view option
        this.ViewData["ViewOption"] = this.settings.Value.ListView;

        this.ViewData[GlobalConstants.TotalPostCount] = await posts.CountAsync().ConfigureAwait(true);
        this.ViewData[GlobalConstants.Title] = $"{this.manifest.Name} {tag}";
        this.ViewData[GlobalConstants.Description] = $"Articles posted in the {tag} tag";
        this.ViewData[GlobalConstants.prev] = $"/blog/tag/{tag}/{page + 1}/";
        this.ViewData[GlobalConstants.next] = $"/blog/tag/{tag}/{(page <= 1 ? null : page - 1 + "/")}";
        return this.View("~/Views/Home/Index.cshtml", filteredPosts.AsAsyncEnumerable());
    }

    [Route("/blog/comment/{postId}/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(string postId, string commentId)
    {
        var post = await this.blog.GetPostById(postId).ConfigureAwait(false);

        if (post is null)
        {
            return this.NotFound();
        }

        var comment = post.Comments.FirstOrDefault(c => c.Id.Equals(commentId, StringComparison.OrdinalIgnoreCase));

        if (comment is null)
        {
            return this.NotFound();
        }

        post.Comments.Remove(comment);
        await this.blog.SavePost(post).ConfigureAwait(false);

        return this.Redirect($"{post.GetEncodedLink()}#comments");
    }

    
    [HttpPost]
    [Authorize]
    [AutoValidateAntiforgeryToken][Route("/blog/deletepost/{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        var existing = await this.blog.GetPostById(id).ConfigureAwait(false);
        if (existing is null)
        {
            return this.NotFound();
        }

        await this.blog.DeletePost(existing).ConfigureAwait(false);
        return this.Redirect("/");
    }


    [HttpGet]
    [Authorize]
    [Route("/blog/edit/{id?}")]
    public async Task<ActionResult> Edit(string? id, IFormFile mediaUrl)
    {
        Post =
        var categories = await this.blog.GetCategories().ToListAsync();
        categories.Sort();
        this.ViewData[GlobalConstants.AllCats] = categories;

        var tags = await this.blog.GetTags().ToListAsync();
        tags.Sort();
        this.ViewData[GlobalConstants.AllTags] = tags;

        if (string.IsNullOrEmpty(id))
        {
            return this.View(new Post());
        }
        

        var name = Guid.NewGuid().C + Path.GetExtension(mediaUrl.FileName);
        if (mediaUrl != null)
        {
            var url = await this.cloudinaryService.UploadMediaAsync(
                mediaUrl,
                name,
                "blog_media");
        }

        var post = await this.blog.GetPostById(id).ConfigureAwait(false);

        return post is null ? this.NotFound() : (IActionResult)this.View(post);
    }

    /// <remarks>This is for redirecting potential existing URLs from the old Miniblog URL format.</remarks>
    [Route("/post/{slug}")]
    [HttpGet]
    public IActionResult Redirects(string slug) => this.LocalRedirectPermanent($"/blog/{slug}");

    [Route("/blog/{slug?}")]
    [HttpPost, Authorize, AutoValidateAntiforgeryToken]
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Consumer preference.")]
    public async Task<IActionResult> UpdatePost(Post post)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(nameof(Edit), post);
        }

        if (post == null)
        {
            throw new ArgumentNullException(nameof(post));
        }

        var postById = await this.blog.GetPostById(post.Id).ConfigureAwait(false);

        if (postById == null)
        {
            return this.NotFound();
        }

        var existing = postById ?? post;

        string categories = this.Request.Form[GlobalConstants.categories];
        string tags = this.Request.Form[GlobalConstants.tags];

        existing.Categories.Clear();
        categories.Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim().ToLowerInvariant())
            .ToList()
            .ForEach(existing.Categories.Add);
        existing.Tags.Clear();
        tags.Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim().ToLowerInvariant())
            .ToList()
            .ForEach(existing.Tags.Add);
        existing.Title = post.Title.Trim();
        existing.Slug = !string.IsNullOrWhiteSpace(post.Slug) ? post.Slug.Trim() : Scribere.Data.Models.Post.CreateSlug(post.Title);
        existing.IsPublished = post.IsPublished;
        existing.Content = post.Content.Trim();
        existing.Excerpt = post.Excerpt.Trim();
        
        await this.SaveFilesToDisk(existing).ConfigureAwait(false);

        await this.blog.SavePost(existing).ConfigureAwait(false);

        return this.Redirect(post.GetEncodedLink());
    }

    private async Task SaveFilesToDisk(Post post)
    {
        var imgRegex = new Regex("<img[^>]+ />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        var base64Regex = new Regex("data:[^/]+/(?<ext>[a-z]+);base64,(?<base64>.+)", RegexOptions.IgnoreCase);
        var allowedExtensions = new[] {
            ".jpg",
            ".jpeg",
            ".gif",
            ".png",
            ".webp"
        };

        foreach (Match? match in imgRegex.Matches(post.Content))
        {
            if (match is null)
            {
                continue;
            }

            var doc = new XmlDocument();
            doc.LoadXml($"<root>{match.Value}</root>");

            var img = doc.FirstChild.FirstChild;
            var srcNode = img.Attributes["src"];
            var fileNameNode = img.Attributes["data-filename"];

            // The HTML editor creates base64 DataURIs which we'll have to convert to image
            // files on disk
            if (srcNode is null || fileNameNode is null)
            {
                continue;
            }

            var extension = System.IO.Path.GetExtension(fileNameNode.Value);

            // Only accept image files
            if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var base64Match = base64Regex.Match(srcNode.Value);
            if (base64Match.Success)
            {
                var bytes = Convert.FromBase64String(base64Match.Groups["base64"].Value);
                srcNode.Value = await this.blog.SaveFile(bytes, fileNameNode.Value).ConfigureAwait(false);

                img.Attributes.Remove(fileNameNode);
                post.Content = post.Content.Replace(match.Value, img.OuterXml, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}