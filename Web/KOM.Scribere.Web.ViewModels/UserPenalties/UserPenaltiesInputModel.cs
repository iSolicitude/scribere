namespace KOM.Scribere.Web.ViewModels.UserPenalties;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

public class UserPenaltiesInputModel
{
    [Required]
    public string UserId { get; set; }

    [MaxLength(200)]
    [Display(Name = "Reason to be blocked")]
    public string ReasonToBeBlocked { get; set; }

    public IEnumerable<SelectListItem> BlockedUsernames { get; set; }

    public IEnumerable<SelectListItem> UnblockedUsernames { get; set; }
}
