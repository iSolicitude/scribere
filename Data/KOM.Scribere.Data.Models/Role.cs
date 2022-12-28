// ReSharper disable VirtualMemberCallInConstructor
namespace KOM.Scribere.Data.Models;

using System;

using KOM.Scribere.Data.Common.Models;

using Microsoft.AspNetCore.Identity;

public class Role : IdentityRole, IAuditInfo, IDeletableEntity
{
    public Role()
        : this(null)
    {
    }

    public Role(string name)
        : base(name)
    {
        this.Id = Guid.NewGuid().ToString();
    }

    public DateTime CreatedOn { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedOn { get; set; }
}