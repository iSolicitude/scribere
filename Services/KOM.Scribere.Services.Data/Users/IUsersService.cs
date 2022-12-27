namespace KOM.Scribere.Services.Data.Users;

using System.Collections.Generic;
using System.Threading.Tasks;

using KOM.Scribere.Web.ViewModels.Users;

public interface IUsersService
{
    Task<AllUsersViewModel> GetAllUsersAsync();

    Task<IEnumerable<BannedApplicationUserViewModel>> GetAllBannedUsersAsync();

    Task<UsernamesRolesIndexViewModel> GetUsernamesRolesAsync();

    Task<bool> IsUserAlreadyAddedInRoleAsync(string inputUserId, string inputRoleId);

    Task<bool> UpdateUserRoleAsync(string userId, string inputRoleId);
}
