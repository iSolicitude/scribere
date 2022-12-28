namespace KOM.Scribere.Services.Data.Profile;

using System.Threading.Tasks;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Web.ViewModels.Profile;

public interface IProfileService
{
    Task<string> GetPersonalDataForUserJsonAsync(string userId);

    Task<bool> DeleteUserAsync(User user);

    Task<UserEditInputModel> GetDetailsForUserEditAsync(string userId);

    Task<bool> EditUserDetailsAsync(User user, UserEditInputModel inputModel);

    Task<AllUsersListViewModel> GetAllUsersExceptAdminsAsync(int page, int take, int skip = 0);
}