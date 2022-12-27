using System;
using AutoMapper;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Services.Mapping;

namespace KOM.Scribere.Web.ViewModels.Users;

public class BannedApplicationUserViewModel : IMapFrom<User>, IHaveCustomMappings
{
    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName => $"{this.FirstName} {this.LastName}";

    public string Role { get; set; }

    public DateTime BannedOn { get; set; }

    public string Town { get; set; }

    public string CountryName { get; set; }

    public string Address { get; set; }

    public string ReasonToBeBlocked { get; set; }

    public void CreateMappings(IProfileExpression configuration)
    {
        configuration.CreateMap<User, BannedApplicationUserViewModel>()
            .ForMember(dest => dest.BannedOn, opt => opt.MapFrom(src => src.DeletedOn));
    }
}