using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Extensions;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddDbContextCheck<TContext>(
        this IHealthChecksBuilder builder,
        string name = default!,
        HealthStatus? failureStatus = default,
        IEnumerable<string> tags = default!)
        where TContext : DbContext
    {
        return builder.AddCheck<DbContextHealthCheck<TContext>>(
            name ?? typeof(TContext).Name,
            failureStatus,
            tags);
    }
}

public class DbContextHealthCheck<TContext> : IHealthCheck
    where TContext : DbContext
{
    private readonly TContext _context;

    public DbContextHealthCheck(TContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}