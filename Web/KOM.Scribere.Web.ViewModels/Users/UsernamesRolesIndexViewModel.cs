using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KOM.Scribere.Web.ViewModels.Users;

public class UsernamesRolesIndexViewModel
{
    [Required]
    [Display(Name = "Role")]
    public string RoleId { get; set; }

    public IEnumerable<SelectListItem> Roles { get; set; }

    [Required]
    [Display(Name = "Username")]
    public string UserId { get; set; }

    public IEnumerable<SelectListItem> Users { get; set; }
}