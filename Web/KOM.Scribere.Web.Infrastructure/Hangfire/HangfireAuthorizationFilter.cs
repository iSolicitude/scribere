using Hangfire.Dashboard;

namespace KOM.Scribere.Web.Infrastructure.Hangfire;

using KOM.Scribere.Common;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.IsInRole(GlobalConstants.AdministratorRoleName);
    }
}
