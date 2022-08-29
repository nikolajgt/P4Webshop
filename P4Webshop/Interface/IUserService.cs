

using P4Webshop.Interface.Generic;
using P4Webshop.Models;
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;

namespace P4Webshop.Interface
{
    public interface IUserService
    {
        IGenericUserRepository<Customer> _genericCustomerRepository { get; set; }
        IGenericUserRepository<Admin> _genericAdminRepository { get; set; }
        IGenericUserRepository<Employee> _genericEmployeeRepository { get; set; }
        Task<AuthenticateResponse> GenerateTokensAsync(Guid id, Roles role, string username, string ipaddress);
        //Sets active token to not active and gets new refresh and jwt
        Task<AuthenticateResponse> NewRefreshTokenAsync(Guid id, Roles role, string oldRefreshToken, string ipaddress);

        //Sets active token to not active
        Task<bool> RevokeTokenAsync(Guid id, Roles role, string token, string ipadress);
        Task<User> GetUserByIDAsync(Roles role, Guid id);
        Task SaveAsync();
        Task Dispose();
        bool IsHealthy();
    }
}
