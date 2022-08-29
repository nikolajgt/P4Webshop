using P4Webshop.Interface;
using P4Webshop.Interface.Generic;
using P4Webshop.Models;
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;
using P4Webshop.Repositorys;

namespace P4Webshop.Services
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        public IGenericUserRepository<Customer> _genericCustomerRepository { get; set; }
        public IGenericUserRepository<Admin> _genericAdminRepository { get; set; }
        public IGenericUserRepository<Employee> _genericEmployeeRepository { get; set; }

        private ILogger _logger;
        private IJwtService _jwtService;

        public UserService(MyDbContext context, ILoggerFactory loggerFactory, IJwtService jwtExtension)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logss");
            _jwtService = jwtExtension;
            _genericCustomerRepository = new GenericUserRepository<Customer>(context, _logger);
            _genericAdminRepository = new GenericUserRepository<Admin>(context, _logger);
            _genericEmployeeRepository = new GenericUserRepository<Employee>(context, _logger);
        }

        public async Task<AuthenticateResponse> GenerateTokensAsync(Guid id, Roles role, string username, string ipaddress)
        {
            try
            {
                switch (role)
                {
                    case Roles.Customer:
                        Customer customer = await _genericCustomerRepository.GetUserEntityByIDAsync(id);

                        var customerToken = _jwtService.GenerateJwtToken(role, id, username);
                        var customerRefreshToken = _jwtService.GenerateRefreshToken(ipaddress);
                        if(customer.RefreshTokens == null)
                            customer.RefreshTokens = new List<RefreshToken>();

                        customer.RefreshTokens.Add(customerRefreshToken);
                        await _genericCustomerRepository.UpdateUserEntityAsync(customer);

                        return new AuthenticateResponse(customer, customerToken, customerRefreshToken.Token);

                    case Roles.Admin:
                        Admin admin = await _genericAdminRepository.GetUserEntityByIDAsync(id);

                        var adminToken = _jwtService.GenerateJwtToken(role, id, username);
                        var adminRefreshToken = _jwtService.GenerateRefreshToken(ipaddress);
                        if (admin.RefreshTokens == null)
                            admin.RefreshTokens = new List<RefreshToken>();

                        admin.RefreshTokens.Add(adminRefreshToken);
                        await _genericAdminRepository.UpdateUserEntityAsync(admin);

                        return new AuthenticateResponse(admin, adminToken, adminRefreshToken.Token);

                    case Roles.Employee:
                        Employee employee = await _genericEmployeeRepository.GetUserEntityByIDAsync(id);

                        var employeeToken = _jwtService.GenerateJwtToken(role, id, username);
                        var employeeRefreshToken = _jwtService.GenerateRefreshToken(ipaddress);
                        if (employee.RefreshTokens == null)
                            employee.RefreshTokens = new List<RefreshToken>();

                        employee.RefreshTokens.Add(employeeRefreshToken);
                        await _genericEmployeeRepository.UpdateUserEntityAsync(employee);

                        return new AuthenticateResponse(employee, employeeToken, employeeRefreshToken.Token);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{UserService} GenerateTokens error", typeof(UserService));
                return null;
            }
        }

        public async Task<AuthenticateResponse> NewRefreshTokenAsync(Guid id, Roles role, string oldRefreshToken, string ipaddress)
        {
            try
            {
                switch(role)
                {
                    case Roles.Customer:
                        Customer customer = await _genericCustomerRepository.RefreshTokenUserEntity(oldRefreshToken);
                        if (customer is null)
                            return null;

                        var customerRefreshToken = customer.RefreshTokens.SingleOrDefault(x => x.Token == oldRefreshToken);
                        if (!customerRefreshToken.IsActive)
                            return null;

                        var newCustomerRefreshToken = _jwtService.GenerateRefreshToken(ipaddress);
                        customerRefreshToken.Revoked = DateTime.Now;
                        customerRefreshToken.RevokedByIp = ipaddress;
                        customerRefreshToken.ReplaceByToken = newCustomerRefreshToken.Token;

                        customer.RefreshTokens.Add(customerRefreshToken);
                        var customerResponse = await _genericCustomerRepository.UpdateUserEntityAsync(customer);
                        var customerJwt = _jwtService.GenerateJwtToken(customer.Roles, customer.Id, customer.UserName);

                        if (!customerResponse)
                            return null;

                        return new AuthenticateResponse(customer, customerJwt, newCustomerRefreshToken.Token);

                    case Roles.Admin:
                        Admin admin = await _genericAdminRepository.RefreshTokenUserEntity(oldRefreshToken);
                        if (admin is null)
                            return null;

                        var adminRefreshToken = admin.RefreshTokens.SingleOrDefault(x => x.Token == oldRefreshToken);
                        if (!adminRefreshToken.IsActive)
                            return null;

                        var newAdminRefreshToken = _jwtService.GenerateRefreshToken(ipaddress);
                        adminRefreshToken.Revoked = DateTime.Now;
                        adminRefreshToken.RevokedByIp = ipaddress;
                        adminRefreshToken.ReplaceByToken = newAdminRefreshToken.Token;

                        admin.RefreshTokens.Add(adminRefreshToken);
                        var adminResponse = await _genericAdminRepository.UpdateUserEntityAsync(admin);
                        var adminJwt = _jwtService.GenerateJwtToken(admin.Roles, admin.Id, admin.UserName);

                        if (!adminResponse)
                            return null;

                        return new AuthenticateResponse(admin, adminJwt, newAdminRefreshToken.Token);

                    case Roles.Employee:
                        Employee employee = await _genericEmployeeRepository.RefreshTokenUserEntity(oldRefreshToken);
                        if (employee is null)
                            return null;

                        var employeeRefreshToken = employee.RefreshTokens.SingleOrDefault(x => x.Token == oldRefreshToken);
                        if (!employeeRefreshToken.IsActive)
                            return null;

                        var newEmployeeRefreshToken = _jwtService.GenerateRefreshToken(ipaddress);
                        employeeRefreshToken.Revoked = DateTime.Now;
                        employeeRefreshToken.RevokedByIp = ipaddress;
                        employeeRefreshToken.ReplaceByToken = newEmployeeRefreshToken.Token;

                        employee.RefreshTokens.Add(employeeRefreshToken);
                        var employeeResponse = await _genericEmployeeRepository.UpdateUserEntityAsync(employee);
                        var employeeJwt = _jwtService.GenerateJwtToken(employee.Roles, employee.Id, employee.UserName);

                        if (!employeeResponse)
                            return null;

                        return new AuthenticateResponse(employee, employeeJwt, newEmployeeRefreshToken.Token);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{UserService} NewRefreshTokenAsync error", typeof(UserService));
                return null;
            }
        }

        public async Task<bool> RevokeTokenAsync(Guid id, Roles role, string token, string ipadress)
        {
            try
            {
                switch(role)
                {
                    case Roles.Customer:
                        Customer customer = await _genericCustomerRepository.RefreshTokenUserEntity(token);
                        if (customer == null)
                            return false;

                        var customerRefreshToken = customer.RefreshTokens.SingleOrDefault(x => x.Token == token);
                        if (!customerRefreshToken.IsActive)
                            return false;

                        customerRefreshToken.Revoked = DateTime.UtcNow;
                        customerRefreshToken.RevokedByIp = ipadress;

                        customer.RefreshTokens.Add(customerRefreshToken);

                        return await _genericCustomerRepository.UpdateUserEntityAsync(customer);

                    case Roles.Admin:
                        Admin admin = await _genericAdminRepository.RefreshTokenUserEntity(token);
                        if (admin == null)
                            return false;

                        var adminRefreshToken = admin.RefreshTokens.SingleOrDefault(x => x.Token == token);
                        if (!adminRefreshToken.IsActive)
                            return false;

                        adminRefreshToken.Revoked = DateTime.UtcNow;
                        adminRefreshToken.RevokedByIp = ipadress;

                        admin.RefreshTokens.Add(adminRefreshToken);

                        return await _genericAdminRepository.UpdateUserEntityAsync(admin);

                    case Roles.Employee:
                        Employee employee = await _genericEmployeeRepository.RefreshTokenUserEntity(token);
                        if (employee == null)
                            return false;

                        var employeeRefreshToken = employee.RefreshTokens.SingleOrDefault(x => x.Token == token);
                        if (!employeeRefreshToken.IsActive)
                            return false;

                        employeeRefreshToken.Revoked = DateTime.UtcNow;
                        employeeRefreshToken.RevokedByIp = ipadress;

                        employee.RefreshTokens.Add(employeeRefreshToken);

                        return await _genericEmployeeRepository.UpdateUserEntityAsync(employee);
                }
                return false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{UserService} RevokeTokenAsync error", typeof(UserService));
                return false;
            }
        }

        public async Task<User> GetUserByIDAsync(Roles role, Guid id)
        {
            try
            {
                switch (role)
                {
                    case Roles.Customer:
                        return await _genericCustomerRepository.GetUserEntityByIDAsync(id);

                    case Roles.Admin:
                        return await _genericAdminRepository.GetUserEntityByIDAsync(id);

                    case Roles.Employee:
                        return await _genericEmployeeRepository.GetUserEntityByIDAsync(id);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{AdminService} GetUserByIDAsync error", typeof(AdminService));
                return null;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public bool IsHealthy()
        {
            return true;
        }


        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}
