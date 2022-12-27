namespace KOM.Scribere.Web.Areas.Administration.Controllers;

using KOM.Scribere.Common;
using KOM.Scribere.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = GlobalConstants.AdministratorRoleName)]
[Area("Administration")]
public class AdministrationController : BaseController
{
}