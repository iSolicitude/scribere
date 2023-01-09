// ReSharper disable VirtualMemberCallInConstructor

namespace KOM.Scribere.Data.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using KOM.Scribere.Data.Common.Models;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser, IAuditInfo, IDeletableEntity
{
    public User()
    {
        this.Id = Guid.NewGuid().ToString();
        this.Roles = new HashSet<IdentityUserRole<string>>();
        this.Claims = new HashSet<IdentityUserClaim<string>>();
        this.Logins = new HashSet<IdentityUserLogin<string>>();
    }

    /// <summary>
    /// Gets or sets user first name
    /// </summary>
    [PersonalData]
    [MaxLength(25, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [Display(Name = "FirstName")]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets user last name
    /// </summary>
    [PersonalData]
    [MaxLength(25, ErrorMessage = "The {0} must be at least {1} characters long.")]
    [Display(Name = "LastName")]
    public string LastName { get; set; }

    /// <summary>
    /// Combined first name and last name
    /// </summary>
    /// <returns>Fullname</returns>
    public override string ToString()
    {
        return $"{this.FirstName} {this.LastName}";
    }

    /// <summary>
    /// Gets or sets get image from url
    /// </summary>
    [PersonalData]
    public string ImgUrl { get; set; } = "~/assets/img/elements-ui-users/UserEFDefaults.svg";

    /// <summary>
    /// Gets or sets a value indicating whether user account is activated
    /// </summary>
    public bool IsBlocked { get; set; }

    [MaxLength(200)]
    public string ReasonToBeBlocked { get; set; }

    /// <summary>
    /// Gets or sets which admin the user was activated by.
    /// </summary>
    public string ActivatedBy { get; set; }

    /// <summary>
    /// Gets or sets the last page the user visited.
    /// </summary>
    [Display(Name = "Last Page Visited")]
    public string LastPageVisited { get; set; } = "/";

    /// <summary>
    /// Gets or sets user counts.
    /// </summary>
    public int Count { get; set; } = 0;

    // Audit info
    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    // Deletable entity
    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

    public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

    public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
}
