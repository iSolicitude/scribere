using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KOM.Scribere.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Web.Infrastructure.ValidationAttributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KOM.Scribere.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly SignInManager<User> signInManager;
    private readonly RoleManager<Role> roleManager;
    private readonly IDeletableEntityRepository<User> usersRepository;
    private readonly UserManager<User> userManager;
    private readonly ILogger<RegisterModel> logger;

    public RegisterModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IDeletableEntityRepository<User> usersRepository,
        ILogger<RegisterModel> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.usersRepository = usersRepository;
        this.logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required.")]
        [StringLength(15, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
        [StringLength(20, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required.")]
        [StringLength(20, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        [StringLength(20, ErrorMessage = "The {0} should be between {2} and {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [GoogleReCaptchaValidation]
        public string RecaptchaValue { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        this.ReturnUrl = returnUrl;
        this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= this.Url.Content("~/");
        this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (this.ModelState.IsValid)
        {
            var isUserNameExisting = await this.usersRepository
                .AllAsNoTrackingWithDeleted()
                .AnyAsync(u => u.UserName == this.Input.UserName);

            if (isUserNameExisting == false)
            {
                var user = new User
                {
                    UserName = this.Input.UserName,
                    Email = this.Input.Email,
                    FirstName = this.Input.FirstName,
                    LastName = this.Input.LastName,
                };

                var result = await this.userManager.CreateAsync(user, this.Input.Password);

                if (result.Succeeded)
                {
                    Role role = await this.roleManager.FindByNameAsync(GlobalConstants.UserRoleName);

                    await this.userManager.AddToRoleAsync(user, role.Name);

                    this.logger.LogInformation("User created a new account with password.");

                    await this.signInManager.SignInAsync(user, isPersistent: false);

                    return this.LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, NotificationMessages.UsernameIsTaken);
            }
        }

        // If we got this far, something failed, redisplay form
        return this.Page();
    }
}