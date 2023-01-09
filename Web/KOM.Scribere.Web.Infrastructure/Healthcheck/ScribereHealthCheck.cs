namespace KOM.Scribere.Web.Infrastructure.Healthcheck;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public class ScribereHealthCheck : IHealthCheck
{
    private Random random = new Random();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var responseTime = this.random.Next(1, 300);

        if (responseTime < 100)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Healthy result from MyHealthCheck"));
        }
        else if (responseTime < 200)
        {
            return Task.FromResult(HealthCheckResult.Degraded("Degraded result from MyHealthCheck"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Unhealthy result from MyHealthCheck"));
    }
}
