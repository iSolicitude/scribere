using KOM.Scribere.Services.Data.Notifications;

namespace KOM.Scribere.Services.Data.Profile;

using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using KOM.Scribere.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;
using KOM.Scribere.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ProfileService : IProfileService
{
    private readonly IDeletableEntityRepository<User> usersRepository;

    private readonly IDeletableEntityRepository<Role> rolesRepository;

    private readonly IDeletableEntityRepository<Comment> _comments;

    private readonly INotificationsService notificationsService;

    private readonly UserManager<User> userManager;

    public ProfileService(
        IDeletableEntityRepository<User> usersRepository,
        IDeletableEntityRepository<Role> rolesRepository,
        IDeletableEntityRepository<Comment> comments,
        INotificationsService notificationsService,
        UserManager<User> userManager)
    {
        this.usersRepository = usersRepository;
        this.rolesRepository = rolesRepository;
        this.userManager = userManager;
        this._comments = comments;
        this.notificationsService = notificationsService;
    }

    public async Task<bool> DeleteUserAsync(User user)
    {
        try
        {
            this._comments.Delete(null);
            await this.notificationsService.DeleteAllNotificationsByUserIdAsync(user.Id, user.UserName);

            this.usersRepository.Delete(user);

            await this.usersRepository.SaveChangesAsync();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<bool> EditUserDetailsAsync(User user, UserEditInputModel inputModel)
    {
        user.FirstName = inputModel.FirstName;
        user.LastName = inputModel.LastName;
        user.UserName = inputModel.UserName;
        user.PhoneNumber = inputModel.PhoneNumber;

        var result = await this.userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<AllUsersListViewModel> GetAllUsersExceptAdminsAsync(int page, int take, int skip = 0)
    {
        var adminRole = await this.rolesRepository
            .AllAsNoTracking()
            .Select(r => new
            {
                r.Id,
                r.Name,
            })
            .SingleOrDefaultAsync(r => r.Name == GlobalConstants.AdministratorRoleName);

        var users = await this.usersRepository
            .AllAsNoTracking()
            .Where(u => u.Roles.Any(r => r.RoleId == adminRole.Id) == false)
            .OrderBy(u => u.UserName)
            .Skip(skip)
            .Take(take)
            .To<UserInListViewModel>()
            .ToListAsync();

        var usersCount = await this.usersRepository.AllAsNoTracking().CountAsync();

        var viewModel = new AllUsersListViewModel
        {
            Users = users,
            CurrentPage = page,
            ItemsCount = usersCount,
            ItemsPerPage = take,
        };

        return viewModel;
    }

    public async Task<UserEditInputModel> GetDetailsForUserEditAsync(string userId)
    {
        var viewModel = await this.usersRepository
            .All()
            .Where(u => u.Id == userId)
            .To<UserEditInputModel>()
            .SingleOrDefaultAsync();

        return viewModel;
    }

    public async Task<string> GetPersonalDataForUserJsonAsync(string userId)
    {
        if (userId == null)
        {
            return null;
        }

        var user = await this.usersRepository
            .AllAsNoTracking()
            .Where(u => u.Id == userId)
            .To<DownloadPersonalDataViewModel>()
            .SingleOrDefaultAsync();

        if (user == null)
        {
            return null;
        }

        var personalData = new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.CreatedOn,

            Comments = user.Comments.Select(c => new
                {
                    c.CreatedOn,
                    c.Content,
                })
                .ToArray(),
            FavoriteProduct = user.FavoriteProducts.Select(fp => new
                {
                    fp.Id,
                    fp.CreatedOn,
                    fp.PostName,
                })
                .ToArray(),
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        var json = JsonSerializer.Serialize(personalData, options);

        return json;
    }
}