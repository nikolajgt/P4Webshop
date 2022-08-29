using P4Webshop.Interface;
using P4Webshop.Models.HTTP;
using System.Diagnostics;

namespace P4Webshop.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private Stopwatch sw;

        private readonly string baseAddress = "https://localhost:7299/api/HealthCheck/";

        public HttpClientService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            _client = httpClientFactory.CreateClient();
            _logger = loggerFactory.CreateLogger<HttpClientService>();
        }

        /// <summary>
        /// DUMMY FUNCTION
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseHealthCheck> HealthCheckEndpointAsync(CancellationToken cancellationToken, string endpoint)
        {
            try
            {
                sw = Stopwatch.StartNew();
                var response = await _client.GetAsync(baseAddress + endpoint, cancellationToken);
                sw.Stop();

                return new HttpResponseHealthCheck
                {
                    HttpResponseMessage = response,
                    Stopwatch = sw,
                };
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, $"HttpClient service failed");
                return null;
            }
        }

    }
}
