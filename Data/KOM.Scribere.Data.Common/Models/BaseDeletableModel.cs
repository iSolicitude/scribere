namespace KOM.Scribere.Data.Common.Models;

using System;

public abstract class BaseDeletableModel<TKey> : BaseModel<TKey>, IDeletableEntity
{
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedOn { get; set; }
}