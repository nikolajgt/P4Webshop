using Microsoft.Extensions.Diagnostics.HealthChecks;
using P4Webshop.Interface;

namespace P4Webshop.Extensions.HealthChecks
{
    public class UserServiceHealthCheck : IHealthCheck
    {
        private IServiceScopeFactory _scopeFactory;
        private readonly string end_point = "check-userservice-health";

        public UserServiceHealthCheck(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var http = scope.ServiceProvider.GetService<IHttpClientService>();
                var response = await http.HealthCheckEndpointAsync(cancellationToken, end_point);

                if (response == null)
                    return HealthCheckResult.Unhealthy($"Response is null");

                if (response.HttpResponseMessage.IsSuccessStatusCode)
                {
                    if (response.Stopwatch.ElapsedMilliseconds < 100)
                    {
                        return HealthCheckResult.Healthy($"The response time is good {response.Stopwatch.Elapsed}");
                    }
                    else if (response.Stopwatch.ElapsedMilliseconds < 300)
                    {
                        return HealthCheckResult.Degraded($"The response time is Degraded {response.Stopwatch.Elapsed}");
                    }
                    else if (response.Stopwatch.ElapsedMilliseconds < 500)
                    {
                        return HealthCheckResult.Unhealthy($"The response time is Unhealthy {response.Stopwatch.Elapsed}");
                    }
                }
            }
            return HealthCheckResult.Unhealthy($"The response hit, but is not succesfull");
        }
    }
}
