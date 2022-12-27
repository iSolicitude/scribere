namespace KOM.Scribere.Services.Data.UserPenalties;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

public interface IUserPenaltiesService
{
    Task<bool> BlockUserAsync(string userId, string reasonToBeBlocked);

    Task<bool> UnblockUserAsync(string userId);

    Task<IEnumerable<SelectListItem>> GetAllBlockedUsersAsync();

    Task<IEnumerable<SelectListItem>> GetAllUnblockedUsersAsync();
}
