namespace KOM.Scribere.Web.ViewModels.Profile;

using System.ComponentModel.DataAnnotations;

public class ProfileViewModel
{
    [Display(Name = "Full name")]
    public string FullName { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public string City { get; set; }

    public string PostalCode { get; set; }

    public string Phone { get; set; }

    public string GroupId { get; set; }
}