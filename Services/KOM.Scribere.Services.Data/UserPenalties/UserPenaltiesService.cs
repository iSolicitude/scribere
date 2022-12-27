namespace KOM.Scribere.Services.Data.UserPenalties;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KOM.Scribere.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

public class UserPenaltiesService : IUserPenaltiesService
{
    private readonly IDeletableEntityRepository<User> usersRepository;

    private readonly UserManager<User> userManager;

    public UserPenaltiesService(
        IDeletableEntityRepository<User> usersRepository,
        UserManager<User> userManager)
    {
        this.usersRepository = usersRepository;
        this.userManager = userManager;
    }

    public async Task<bool> BlockUserAsync(string userId, string reasonToBeBlocked)
    {
        var user = await this.usersRepository
            .All()
            .SingleOrDefaultAsync(u => u.Id == userId && u.IsBlocked == false);

        if (user != null)
        {
            user.IsBlocked = true;
            user.ReasonToBeBlocked = reasonToBeBlocked ?? NotificationMessages.BannedUserDefaultMessage;
            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;

            var userResult = await this.usersRepository.SaveChangesAsync();

            var logoutResult = await this.userManager.UpdateSecurityStampAsync(user);

            return userResult > 0 && logoutResult.Succeeded;
        }

        return false;
    }

    public async Task<bool> UnblockUserAsync(string userId)
    {
        var affectedRows = await this.usersRepository
            .AllWithDeleted()
            .Where(u => u.Id == userId && u.IsBlocked == true)
            .UpdateAsync(u => new User
            {
                IsBlocked = false,
                ReasonToBeBlocked = null,
                IsDeleted = false,
                DeletedOn = null,
            });

        return affectedRows == 1;
    }

    public async Task<IEnumerable<SelectListItem>> GetAllBlockedUsersAsync()
    {
        var blockedUsersUsernames = await this.usersRepository
            .AllWithDeleted()
            .Where(u => u.IsBlocked == true)
            .Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName,
            })
            .ToListAsync();

        return blockedUsersUsernames;
    }

    public async Task<IEnumerable<SelectListItem>> GetAllUnblockedUsersAsync()
    {
        var unblockedUsersUsernames = await this.usersRepository
            .All()
            .Where(u => u.IsBlocked == false)
            .Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName,
            })
            .ToListAsync();

        return unblockedUsersUsernames;
    }
}