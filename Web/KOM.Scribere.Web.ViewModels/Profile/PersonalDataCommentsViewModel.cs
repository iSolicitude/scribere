namespace KOM.Scribere.Web.ViewModels.Profile;

using System;
using System.Collections.Generic;

using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

public class PersonalDataCommentsViewModel : IMapFrom<Comment>
{
    public DateTime CreatedOn { get; set; }

    public string Content { get; set; }
}
