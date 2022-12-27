namespace KOM.Scribere.Web.Controllers;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using KOM.Scribere.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Web.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AuthenticationController : BaseController
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly RoleManager<Role> roleManager;
    private readonly IDeletableEntityRepository<User> usersRepository;
    private readonly ILogger<AuthenticationController> logger;
    private readonly Post cartService;
    private readonly Random random;

    public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager,
        IDeletableEntityRepository<User> usersRepository, ILogger<AuthenticationController> logger, Post cartService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.usersRepository = usersRepository;
        this.logger = logger;
        this.cartService = cartService;
        this.random = new Random();
    }

    [HttpPost]
    [Authorize]
    [Route("/logout")]
    public async Task<IActionResult> Logout(string returnUrl = null)
    {
        await this.signInManager.SignOutAsync();
        this.logger.LogInformation("User logged out.");

        if (returnUrl != null)
        {
            return this.LocalRedirect(returnUrl);
        }

        this.Success(NotificationMessages.LoggedOut);

        return this.LocalRedirect("/");
    }

    [HttpPost]
    public IActionResult GoogleLogin(string returnUrl = null)
    {
        returnUrl = returnUrl ?? this.Url.Content("~/");

        if (this.User.Identity.IsAuthenticated)
        {
            return this.LocalRedirect(returnUrl);
        }

        var redirectUrl = this.Url.Action("GoogleRegister", "Authentication", new { returnUrl });
        var properties = this.signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

        return new ChallengeResult("Google", properties);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleRegister(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? this.Url.Content("~/");

        if (this.User.Identity.IsAuthenticated)
        {
            return this.LocalRedirect(returnUrl);
        }

        if (remoteError != null)
        {
            return this.RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var info = await this.signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            return this.RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var result = await this.signInManager.ExternalLoginSignInAsync("Google", info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            return this.LocalRedirect(returnUrl);
        }

        this.ViewData["ReturnUrl"] = returnUrl;

        var isEmailExisting = await this.usersRepository
            .AllAsNoTrackingWithDeleted()
            .AnyAsync(u => u.Email == info.Principal.FindFirstValue(ClaimTypes.Email));

        if (isEmailExisting)
        {
            this.Error(NotificationMessages.DuplicateEmail);

            return this.RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var inputModel = new GoogleLoginInputModel();

        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            inputModel.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
        }

        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
        {
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            var split = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length >= 2)
            {
                inputModel.FirstName = split[0];
                inputModel.LastName = split[split.Length - 1];
            }
            else
            {
                inputModel.FirstName = name;
            }
        }

        // extension method
        var randomNumber = RandomExtensions.NextIntRange(this.random, 0, 10000);

        var username = $"{inputModel.Email.Split("@")[0].Trim()}_{randomNumber}";

        var user = new User
        {
            UserName = username,
            Email = inputModel.Email,
            FirstName = inputModel.FirstName,
            LastName = inputModel.LastName,
        };

        var resultAfterCreate = await this.signInManager.UserManager.CreateAsync(user);

        if (resultAfterCreate.Succeeded)
        {
            resultAfterCreate = await this.signInManager.UserManager.AddLoginAsync(user, info);

            if (resultAfterCreate.Succeeded)
            {
                Role role = await this.roleManager.FindByNameAsync(GlobalConstants.UserRoleName);

                await this.userManager.AddToRoleAsync(user, role.Name);

                this.Success(string.Format(NotificationMessages.RegistrationWelcome, inputModel.FirstName));
                this.logger.LogInformation("User created with Google provider.");

                await this.signInManager.SignInAsync(user, isPersistent: true);
                return this.LocalRedirect(returnUrl);
            }
        }
        else
        {
            this.ModelState.AddModelError(string.Empty, NotificationMessages.DuplicateEmail);

            return this.RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        this.ViewData["ReturnUrl"] = returnUrl;
        return this.View();
    }

    [HttpPost]
    public IActionResult FacebookLogin(string returnUrl = null)
    {
        returnUrl = returnUrl ?? this.Url.Content("~/");

        if (this.User.Identity.IsAuthenticated)
        {
            return this.LocalRedirect(returnUrl);
        }

        var redirectUrl = this.Url.Action("FacebookRegister", "Authentication", new { returnUrl });
        var properties = this.signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
        return new ChallengeResult("Facebook", properties);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> FacebookRegister(FacebookLoginInputModel model, string returnUrl = null)
    {
        returnUrl = returnUrl ?? this.Url.Content("~/");

        if (this.User.Identity.IsAuthenticated)
        {
            return this.LocalRedirect(returnUrl);
        }

        var info = await this.signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            return this.RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var result = await this.signInManager.ExternalLoginSignInAsync("Facebook", info.ProviderKey,
            isPersistent: true, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            return this.LocalRedirect(returnUrl);
        }

        this.ViewData["ReturnUrl"] = returnUrl;

        var isEmailExisting = await this.usersRepository
            .AllAsNoTrackingWithDeleted()
            .AnyAsync(u => u.Email == info.Principal.FindFirstValue(ClaimTypes.Email));

        if (isEmailExisting)
        {
            this.Error(NotificationMessages.DuplicateEmail);

            return this.RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var inputModel = new FacebookLoginInputModel();

        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            inputModel.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
        }

        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
        {
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            var split = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length >= 2)
            {
                inputModel.FirstName = split[0];
                inputModel.LastName = split[split.Length - 1];
            }
            else
            {
                inputModel.FirstName = name;
            }
        }

        // extension method
        var randomNumber = RandomExtensions.NextIntRange(this.random, 0, 10000);

        var username = $"{inputModel.Email.Split("@")[0].Trim()}_{randomNumber}";

        var user = new User
        {
            UserName = username,
            Email = inputModel.Email,
            FirstName = inputModel.FirstName,
            LastName = inputModel.LastName,
        };

        var resultAfterCreate = await this.signInManager.UserManager.CreateAsync(user);

        if (resultAfterCreate.Succeeded)
        {
            resultAfterCreate = await this.signInManager.UserManager.AddLoginAsync(user, info);

            if (resultAfterCreate.Succeeded)
            {
                Role role = await this.roleManager.FindByNameAsync(GlobalConstants.UserRoleName);

                await this.userManager.AddToRoleAsync(user, role.Name);

                this.Success(string.Format(NotificationMessages.RegistrationWelcome, inputModel.FirstName));
                this.logger.LogInformation("User created with Facebook provider.");

                await this.signInManager.SignInAsync(user, isPersistent: true);
                return this.LocalRedirect(returnUrl);
            }
        }

        this.ViewData["ReturnUrl"] = returnUrl;
        return this.View();
    }
}