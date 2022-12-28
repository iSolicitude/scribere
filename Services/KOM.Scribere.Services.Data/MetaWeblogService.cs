using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KOM.Scribere.Common;
using KOM.Scribere.Services.Data.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using WilderMinds.MetaWeblog;

namespace KOM.Scribere.Services.Data;

public class MetaWeblogService : IMetaWeblogProvider
{
    private readonly IBlogService blog;

    private readonly IConfiguration config;

    private readonly IHttpContextAccessor context;

    private readonly IUsersService userServices;

    public MetaWeblogService(
        IBlogService blog,
        IConfiguration config,
        IHttpContextAccessor context,
        IUsersService userServices)
    {
        this.blog = blog;
        this.config = config;
        this.userServices = userServices;
        this.context = context;
    }

    public Task<int> AddCategoryAsync(string key, string username, string password, NewCategory category)
    {
        
        throw new NotImplementedException();
    }

    public Task<string> AddPageAsync(string blogid, string username, string password, Page page, bool publish)
    {
        
        throw new NotImplementedException();
    }

    public async Task<string> AddPostAsync(string blogid, string username, string password, Post post, bool publish)
    {
        
        if (post is null)
        {
            throw new ArgumentNullException(nameof(post));
        }

        var newPost = new Scribere.Data.Models.Post()
        {
            Title = post.title,
            Slug = !string.IsNullOrWhiteSpace(post.wp_slug) ? post.wp_slug : Scribere.Data.Models.Post.CreateSlug(post.title),
            Excerpt = post.mt_excerpt,
            Content = post.description,
            IsPublished = publish
        };

        post.categories.ToList().ForEach(newPost.Categories.Add);
        post.mt_keywords.Split(',').ToList().ForEach(newPost.Tags.Add);

        if (post.dateCreated != DateTime.MinValue)
        {
            newPost.PublishDate = post.dateCreated;
        }

        await this.blog.SavePost(newPost).ConfigureAwait(false);

        return newPost.Id;
    }

    public Task<bool> DeletePageAsync(string blogid, string username, string password, string pageid)
    {
        
        throw new NotImplementedException();
    }

    public async Task<bool> DeletePostAsync(string key, string postid, string username, string password, bool publish)
    {
        
        var post = await this.blog.GetPostById(postid).ConfigureAwait(false);
        if (post is null)
        {
            return false;
        }

        await this.blog.DeletePost(post).ConfigureAwait(false);
        return true;
    }

    public Task<bool> EditPageAsync(string blogid, string pageid, string username, string password, Page page, bool publish)
    {
        
        throw new NotImplementedException();
    }

    public async Task<bool> EditPostAsync(string postid, string username, string password, Post post, bool publish)
    {
        
        var existing = await this.blog.GetPostById(postid).ConfigureAwait(false);

        if (existing is null || post is null)
        {
            return false;
        }

        existing.Title = post.title;
        existing.Slug = post.wp_slug;
        existing.Excerpt = post.mt_excerpt;
        existing.Content = post.description;
        existing.IsPublished = publish;
        existing.Categories.Clear();
        post.categories.ToList().ForEach(existing.Categories.Add);
        existing.Tags.Clear();
        post.mt_keywords.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(existing.Tags.Add);

        if (post.dateCreated != DateTime.MinValue)
        {
            existing.PublishDate = post.dateCreated;
        }

        await this.blog.SavePost(existing).ConfigureAwait(false);

        return true;
    }

    public Task<Author[]> GetAuthorsAsync(string blogid, string username, string password) =>
        throw new NotImplementedException();

    public async Task<CategoryInfo[]> GetCategoriesAsync(string blogid, string username, string password)
    {
        
        return await this.blog.GetCategories()
            .Select(
                cat =>
                    new CategoryInfo
                    {
                        categoryid = cat,
                        title = cat
                    })
            .ToArrayAsync();
    }

    public Task<Page> GetPageAsync(string blogid, string pageid, string username, string password) =>
        throw new NotImplementedException();

    public Task<Page[]> GetPagesAsync(string blogid, string username, string password, int numPages) =>
        throw new NotImplementedException();

    public async Task<Post?> GetPostAsync(string postid, string username, string password)
    {
        
        var post = await this.blog.GetPostById(postid).ConfigureAwait(false);

        return post is null ? null : this.ToMetaWebLogPost(post);
    }

    public async Task<Post[]> GetRecentPostsAsync(string blogid, string username, string password, int numberOfPosts)
    {
        
        return await this.blog.GetPosts(numberOfPosts)
            .Select(this.ToMetaWebLogPost)
            .ToArrayAsync();
    }

    public async Task<Tag[]> GetTagsAsync(string blogid, string username, string password)
    {
        
        return await this.blog.GetTags()
            .Select(
                tag =>
                    new Tag
                    {
                        name = tag
                    })
            .ToArrayAsync();
    }

    public Task<UserInfo> GetUserInfoAsync(string key, string username, string password)
    {
        
        throw new NotImplementedException();
    }

    public Task<BlogInfo[]> GetUsersBlogsAsync(string key, string username, string password)
    {
        
        var request = this.context.HttpContext.Request;
        var url = $"{request.Scheme}://{request.Host}";

        return Task.FromResult(
            new[]
            {
                new BlogInfo
                {
                    blogid ="1",
                    blogName = this.config[GlobalConstants.Name] ?? nameof(MetaWeblogService),
                    url = url,
                },
            });
    }

    public async Task<MediaObjectInfo> NewMediaObjectAsync(string blogid, string username, string password, MediaObject mediaObject)
    {
        
        if (mediaObject is null)
        {
            throw new ArgumentNullException(nameof(mediaObject));
        }

        var bytes = Convert.FromBase64String(mediaObject.bits);
        var path = await this.blog.SaveFile(bytes, mediaObject.name).ConfigureAwait(false);

        return new MediaObjectInfo { url = path };
    }

    private Post ToMetaWebLogPost(Scribere.Data.Models.Post post)
    {
        var request = this.context.HttpContext.Request;
        var url = $"{request.Scheme}://{request.Host}";

        return new Post
        {
            postid = post.Id,
            title = post.Title,
            wp_slug = post.Slug,
            permalink = url + post.GetLink(),
            dateCreated = post.PublishDate.DateTime,
            mt_excerpt = post.Excerpt,
            description = post.Content,
            categories = post.Categories.ToArray(),
            mt_keywords = string.Join(',', post.Tags)
        };
    }
}