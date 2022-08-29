using System.Diagnostics;

namespace P4Webshop.Models.HTTP
{
    public class HttpResponseHealthCheck
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public Stopwatch Stopwatch { get; set; }
    }
}
