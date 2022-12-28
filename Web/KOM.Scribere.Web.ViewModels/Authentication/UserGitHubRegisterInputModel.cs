using System.ComponentModel.DataAnnotations;

namespace KOM.Scribere.Web.ViewModels.Authentication;

public class UserGitHubRegisterInputModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
    [StringLength(20, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
    [Display(Name = "First name")]
    public string FirstName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required.")]
    [StringLength(20, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
    [Display(Name = "Last name")]
    public string LastName { get; set; }
}