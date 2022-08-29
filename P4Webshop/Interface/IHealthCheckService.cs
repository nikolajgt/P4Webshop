namespace P4Webshop.Interface
{
    public interface IHealthCheckService
    {
        public IAdminService _adminService { get; set; }
        public IUserService _userService { get; set; }
        public IJwtService _jwtService { get; set; }
        public IWorkerControlService _workerControlService { get; set; }
        public IHttpClientService _httpClientService { get; set; }

        bool IsHealthy();
    }
}
