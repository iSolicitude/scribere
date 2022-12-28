using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KOM.Scribere.Common;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Data.Profile;
using KOM.Scribere.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KOM.Scribere.Web.Controllers;

[Authorize]
public class ProfileController : BaseController
{
    private const string PersonalDataFileName = "{0}_PersonalData_{1}_{2}.json";

    private const int UsersPerPage = 6;

    private readonly UserManager<User> userManager;

    private readonly SignInManager<User> signInManager;

    private readonly IProfileService profileService;
        
    public ProfileController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IProfileService profileService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.profileService = profileService;
    }

    public async Task<IActionResult> Index(string id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await this.userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return this.NotFound();
        }

        var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserName = this.User.FindFirstValue(ClaimTypes.Name);

        var profileViewModel = new ProfileViewModel
        {
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Username = user.UserName,
            Phone = user.PhoneNumber,
        };

        return this.View(profileViewModel);
    }

    [HttpPost]
    [ActionName("Download")]
    public async Task<IActionResult> DownloadPersonalData(string password)
    {
        var user = await this.userManager.GetUserAsync(this.User);

        var passwordValid = !await this.userManager.HasPasswordAsync(user) ||
                            (password != null &&
                             await this.userManager.CheckPasswordAsync(user, password));

        if (passwordValid == false)
        {
            this.Error(NotificationMessages.InvalidPassword);

            return this.RedirectToAction(nameof(this.Index), new { userId = user.Id });
        }

        var json = await this.profileService.GetPersonalDataForUserJsonAsync(user.Id);

        this.Response.Headers.Add("Content-Disposition", "attachment; filename=" + string.Format(PersonalDataFileName, GlobalConstants.SystemName, user.FirstName, user.LastName));

        return new FileContentResult(Encoding.UTF8.GetBytes(json), "text/json");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(string password)
    {
        var user = await this.userManager.GetUserAsync(this.User);

        var passwordValid = !await this.userManager.HasPasswordAsync(user) ||
                            (password != null &&
                             await this.userManager.CheckPasswordAsync(user, password));

        if (passwordValid == false)
        {
            this.Error(NotificationMessages.InvalidPassword);

            return this.RedirectToAction(nameof(this.Index), new { userId = user.Id });
        }

        var result = await this.profileService.DeleteUserAsync(user);

        if (result == false)
        {
            this.Error(NotificationMessages.AccountDeleteError);

            return this.RedirectToAction(nameof(this.Index), new { userId = user.Id });
        }

        await this.signInManager.SignOutAsync();

        this.Success(NotificationMessages.AccountDeleted);

        return this.RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await this.userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.User.FindFirstValue(ClaimTypes.NameIdentifier)}'.");
        }

        var hasPassword = await this.userManager.HasPasswordAsync(user);

        if (hasPassword == false)
        {
            return this.RedirectToAction(nameof(this.SetPassword));
        }

        var inputModel = new UserChangePasswordInputModel
        {
            Email = user.Email,
        };

        return this.View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(UserChangePasswordInputModel inputModel)
    {
        if (this.ModelState.IsValid == false)
        {
            return this.RedirectToAction(nameof(this.ChangePassword));
        }

        var user = await this.userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.User.FindFirstValue(ClaimTypes.NameIdentifier)}'.");
        }

        var changePasswordResult = await this.userManager.ChangePasswordAsync(user, inputModel.OldPassword, inputModel.NewPassword);

        if (changePasswordResult.Succeeded == false)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            inputModel.Email = user.Email;

            return this.View(inputModel);
        }

        await this.signInManager.RefreshSignInAsync(user);

        this.Success(NotificationMessages.PasswordChanged);

        return this.RedirectToAction(nameof(this.Index), new { id = user.Id });
    }

    [HttpGet]
    public async Task<IActionResult> SetPassword()
    {
        var user = await this.userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound(NotificationMessages.UserNotFound);
        }

        var hasPassword = await this.userManager.HasPasswordAsync(user);

        if (hasPassword)
        {
            return this.RedirectToAction(nameof(this.ChangePassword));
        }

        var inputModel = new UserSetPasswordInputModel
        {
            Email = user.Email,
        };

        return this.View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> SetPassword(UserSetPasswordInputModel inputModel)
    {
        if (this.ModelState.IsValid == false)
        {
            return this.RedirectToAction(nameof(this.SetPassword));
        }

        var user = await this.userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.User.FindFirstValue(ClaimTypes.NameIdentifier)}'.");
        }

        if (await this.userManager.HasPasswordAsync(user))
        {
            return this.RedirectToAction(nameof(this.ChangePassword));
        }

        var addPasswordResult = await this.userManager.AddPasswordAsync(user, inputModel.NewPassword);

        if (addPasswordResult.Succeeded == false)
        {
            foreach (var error in addPasswordResult.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            inputModel.Email = user.Email;

            return this.View(inputModel);
        }

        await this.signInManager.RefreshSignInAsync(user);

        this.Success(NotificationMessages.PasswordSet);

        return this.RedirectToAction(nameof(this.Index), new { id = user.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await this.userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound(NotificationMessages.UserNotFound);
        }

        var inputModel = await this.profileService.GetDetailsForUserEditAsync(user.Id);

        return this.View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserEditInputModel inputModel)
    {
        var user = await this.userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound();
        }

        if (this.ModelState.IsValid == false)
        {
            inputModel.Email = user.Email;
            inputModel.UserName = user.UserName;

            return this.View(inputModel);
        }

        var result = await this.profileService.EditUserDetailsAsync(user, inputModel);

        if (result)
        {
            this.Success(NotificationMessages.ProfileDetailsUpdated);
        }
        else
        {
            this.Error(NotificationMessages.CannotUpdateProfileDetails);
        }

        return this.RedirectToAction(nameof(this.Index), new { id = user.Id });
    }

    public async Task<IActionResult> All(int page = 0)
    {
        page = Math.Max(1, page);
        var skip = (page - 1) * UsersPerPage;

        var usersViewModel = await this.profileService.GetAllUsersExceptAdminsAsync(page, UsersPerPage, skip);

        return this.View(usersViewModel);
    }
}