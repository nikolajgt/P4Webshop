using P4Webshop.Interface;

namespace P4Webshop.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        public IAdminService _adminService { get; set; }
        public IUserService _userService { get; set; }
        public IJwtService _jwtService { get; set; }
        public IWorkerControlService _workerControlService { get; set; }
        public IHttpClientService _httpClientService { get; set; }

        public HealthCheckService(IAdminService adminService, IUserService userService, IJwtService jwtService, IWorkerControlService workerControlService, IHttpClientService httpClientService)
        {
            _adminService = adminService;
            _userService = userService;
            _jwtService = jwtService;
            _workerControlService = workerControlService;
            _httpClientService = httpClientService;
        }
        public bool IsHealthy()
        {
            return true;
        }

    }
}
