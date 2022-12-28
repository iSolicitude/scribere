namespace KOM.Scribere.Data.Common.Models;

using System;

public interface IAuditInfo
{
    DateTime CreatedOn { get; set; }

    DateTimeOffset ModifiedOn { get; set; }
}