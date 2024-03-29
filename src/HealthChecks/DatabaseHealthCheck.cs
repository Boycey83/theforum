using Microsoft.Extensions.Diagnostics.HealthChecks;
using theforum.BusinessLogic;

namespace theforum.HealthChecks;

// Truth is that this healthcheck won't even run if the database
// is unavailable because it'll trip over the NhSessionMiddleware
// when it tries to begin the transaction.
//
// Still, leaving the health check in place for when we get off
// NHibernate.

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly PostService _postService;
    
    public DatabaseHealthCheck(PostService postService) => 
        _postService = postService;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _postService.GetThreadReplies(0);
            return !result.Any() 
                ? HealthCheckResult.Healthy() 
                : HealthCheckResult.Unhealthy("Expected an empty set");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check failed", ex);
        }
    }
}
