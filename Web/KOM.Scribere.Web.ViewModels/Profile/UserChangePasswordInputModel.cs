namespace KOM.Scribere.Web.ViewModels.Profile;

using System.ComponentModel.DataAnnotations;

public class UserChangePasswordInputModel
{
    [EmailAddress]
    public string Email { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "New password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
