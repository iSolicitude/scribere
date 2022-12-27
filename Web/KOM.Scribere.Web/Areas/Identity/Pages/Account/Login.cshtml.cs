namespace KOM.Scribere.Web.Areas.Identity.Pages.Account;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Web.Infrastructure.ValidationAttributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly ILogger<LoginModel> logger;

    public LoginModel(
        SignInManager<User> signInManager,
        ILogger<LoginModel> logger,
        UserManager<User> userManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email or Username field is required.")]
        [Display(Name = "Email or Username")]
        public string EmailOrUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [GoogleReCaptchaValidation]
        public string RecaptchaValue { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(this.ErrorMessage))
        {
            this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
        }

        returnUrl ??= this.Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        this.ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= this.Url.Content("~/");

        if (this.Input.EmailOrUserName.IndexOf('@') > -1)
        {
            // Validate email format
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(emailRegex);

            if (!re.IsMatch(this.Input.EmailOrUserName))
            {
                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }

        if (this.ModelState.IsValid)
        {
            var userName = this.Input.EmailOrUserName;

            if (userName.IndexOf('@') > -1)
            {
                var user = await this.userManager.FindByEmailAsync(this.Input.EmailOrUserName);

                if (user == null)
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    return this.Page();
                }
                else
                {
                    userName = user.UserName;
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await this.signInManager.PasswordSignInAsync(userName, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                this.logger.LogInformation("User logged in.");

                return this.LocalRedirect(returnUrl);
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                return this.Page();
            }
        }

        // If we got this far, something failed, redisplay form
        return this.Page();
    }
}