namespace KOM.Scribere.Web.ViewModels.Profile;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using AutoMapper;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;
using KOM.Scribere.Web.Infrastructure.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

public class UserEditInputModel : IMapFrom<User>, IMapTo<User>
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required.")]

    [StringLength(15, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
    [Display(Name = "Username")]
    public string UserName { get; set; }

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

    [Required(AllowEmptyStrings = false, ErrorMessage = "Town is required.")]
    [StringLength(20, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
    public string Town { get; set; }

    [Display(Name = "Country")]
    public int? CountryId { get; set; }

    public IEnumerable<SelectListItem> Countries { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Postal code is required.")]
    [StringLength(20, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
    [Display(Name ="Postal code")]
    public string PostalCode { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phone number is required.")]
    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required.")]
    [StringLength(30, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
    public string Address1 { get; set; }

    [StringLength(30, ErrorMessage = "{0} should be between {2} and {1} characters long.", MinimumLength = 2)]
    public string Address2 { get; set; }

    [AllowedExtensions]
    [MaxFileSize]
    [DataType(DataType.Upload)]
    [Display(Name = "Profile Picture")]
    public IFormFile AvatarImage { get; set; }
}
