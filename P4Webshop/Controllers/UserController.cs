
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P4Webshop.Interface;
using P4Webshop.Models;
using P4Webshop.Models.Base;
using P4Webshop.Models.DTO;
using System.Security.Claims;


namespace P4Webshop.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController 
    {
        private readonly IUserService _service;

        /// <summary>
        /// These is for IdentityCore
        /// </summary>
        private readonly UserManager<Customer> _customerManager;
        private readonly UserManager<Admin> _adminManager;
        private readonly UserManager<Employee> _employeeManager;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly IMapper _mapper;

        public UserController(UserManager<Customer> customerManager, UserManager<Admin> adminManager, UserManager<Employee> employeeManager, IUserService service, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _customerManager = customerManager;
            _adminManager = adminManager;
            _employeeManager = employeeManager;
            _contextAccessor = contextAccessor;
            _service = service;
            _user = contextAccessor.HttpContext.User;
            _mapper = mapper;
        }

        /// <summary>
        /// Post user is for creating users. It uses IdentityCore w. EFcore for checing existing usernames and email and salts/hashing passwords 
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Post-User")]
        public async Task<IActionResult> PostUser([FromBody] UserDTO u)
        {
            IdentityResult result = new IdentityResult();
            switch (u.Roles)
            {
                case Roles.Customer:
                    Customer newCustomer = new Customer(u.UserName, u.Email, u.Firstname, u.Lastname, u.Balance, u.Roles);
                    result = await _customerManager.CreateAsync(newCustomer, u.Password);
                    break;

                case Roles.Admin:
                    Admin newAdmin = new Admin(u.UserName, u.Email, u.Firstname, u.Lastname, u.Roles);
                    result = await _adminManager.CreateAsync(newAdmin, u.Password);
                    break;

                case Roles.Employee:
                    Employee newEmployee = new Employee(u.UserName, u.Email, u.Firstname, u.Lastname, u.Roles);
                    result = await _employeeManager.CreateAsync(newEmployee, u.Password);
                    break;
            }

            if (!result.Succeeded)
            {
                IdentityErrorDescriber errorDescriber = new IdentityErrorDescriber();
                IdentityError primaryError = result.Errors.FirstOrDefault();

                if (primaryError.Code == nameof(errorDescriber.DuplicateEmail))
                {
                    return new ConflictObjectResult("Email already exist");
                }
                else if (primaryError.Code == nameof(errorDescriber.DuplicateUserName))
                {
                    return new ConflictObjectResult("Username already exist");
                }
                else if (primaryError.Code == nameof(errorDescriber.InvalidUserName))
                {
                    return new ConflictObjectResult("Invalid username");
                }
            }

            return new OkObjectResult(true);
        }


        /// <summary>
        /// Login is for login, surprise. It uses IdentityCore w. EFcore for accessing db, checks if username and password is correct.
        /// We dont need to worry about (un)salting/hashing passwords and it returns user info fx username, id and tokens
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Logins")]
        public async Task<IActionResult> Login([FromBody] UserDTO l)
        {
            string ipAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            bool correctPassword = false;
            bool isLockedout = false;
            User user = new User();

            switch (l.Roles)
            {
                case Roles.Customer:
                    Customer customer = await _customerManager.FindByNameAsync(l.UserName);
                    if (customer == null)
                        return new UnauthorizedObjectResult($"Did not find any customer by {l.UserName}");

                    correctPassword = await _customerManager.CheckPasswordAsync(customer, l.Password);
                    isLockedout = await _customerManager.IsLockedOutAsync(customer);
                    user = customer;
                    break;

                case Roles.Admin:
                    Admin admin = await _adminManager.FindByNameAsync(l.UserName);
                    if (admin == null)
                        return new UnauthorizedObjectResult($"Did not find any admin by {l.UserName}");

                    correctPassword = await _adminManager.CheckPasswordAsync(admin, l.Password);
                    user = admin;
                    break;

                case Roles.Employee:
                    Employee employee = await _employeeManager.FindByNameAsync(l.UserName);
                    if (employee == null)
                        return new UnauthorizedObjectResult($"Did not find any employee by {l.UserName}");

                    correctPassword = await _employeeManager.CheckPasswordAsync(employee, l.Password);
                    isLockedout = await _employeeManager.IsLockedOutAsync(employee);
                    user = employee;
                    break;
            }

            if (!correctPassword)
            {
                return new UnauthorizedObjectResult("Wrong password");
            }
            else if(isLockedout)
            {
                return new UnauthorizedObjectResult("You have benn locked out");
            }
            else
            {
                var auth = await _service.GenerateTokensAsync(user.Id, user.Roles, user.UserName, ipAddress);
                await _service.SaveAsync();
                return new OkObjectResult(auth);
            }
               
            return new UnauthorizedObjectResult("Ths was not supposed to happen, thank you for breaking our login form");
        }


        /// <summary>
        /// Revoke token is for when a user logs out and the active refresh token is active, so it cant be used anymore.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("Revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string token)
        {
            var ipAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var userId = Guid.Parse(_user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value);
            var userRole = (Roles)int.Parse(_user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);

            var response = await _service.RevokeTokenAsync(userId, userRole, token, ipAddress);

            if (!response)
                return new UnauthorizedObjectResult(response);

            return new OkObjectResult(response);
        }


        /// <summary>
        /// RefreshToken is for when a users jwt is expired and they still use the app, instead of they should login again we can take their refreshtoken
        /// see if it comes from correct remote IP and return new jwt token and refreshtoken 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("Refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var ipAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var userId = Guid.Parse(_user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value);
            var userRole = (Roles)int.Parse(_user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);

            var response = await _service.NewRefreshTokenAsync(userId, userRole, refreshToken, ipAddress);
            if (response == null)
                return new UnauthorizedObjectResult(response);

            return new OkObjectResult(response);
        }


    }
}
