using System.Collections.Generic;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KOM.Scribere.Web.Areas.Administration.Controllers;

using KOM.Scribere.Common;
using KOM.Scribere.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = GlobalConstants.AdministratorRoleName)]
[Area("Administration")]
public class AdministrationController<T> : BaseController
{
        private ILogger<T> _loggerInstance;
        private INotyfService _notifyInstance;

        protected INotyfService _notify => _notifyInstance ??= HttpContext.RequestServices.GetService<INotyfService>();

        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
}
