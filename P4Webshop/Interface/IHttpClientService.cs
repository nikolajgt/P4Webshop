
using P4Webshop.Models.HTTP;

namespace P4Webshop.Interface
{
    public interface IHttpClientService
    {
        /// <summary>
        /// Dummy
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HttpResponseHealthCheck> HealthCheckEndpointAsync(CancellationToken cancellationToken, string endpoint);
    }
}
