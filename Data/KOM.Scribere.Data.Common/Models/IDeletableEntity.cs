namespace KOM.Scribere.Data.Common.Models;

using System;

public interface IDeletableEntity
{
    bool IsDeleted { get; set; }

    DateTimeOffset? DeletedOn { get; set; }
}