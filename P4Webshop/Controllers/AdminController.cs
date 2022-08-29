using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P4Webshop.Interface;
using P4Webshop.Models;
using P4Webshop.Models.Base;
using P4Webshop.Models.DTO;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        private readonly IWorkerControlService _workerService;
        private readonly UserManager<Customer> _customerManager;

        public AdminController(IAdminService service, IWorkerControlService workerService, UserManager<Customer> customerManager)
        {
            _customerManager = customerManager;
            _service = service;
            _workerService = workerService;
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(BaseProduct product)
        {
            var response = await _service.AddProductAsync(product);
            if (!response)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }


        /// <summary>
        /// Workers and health checks
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost("services-powermode")]
        public async Task<IActionResult> StopBackgroundWorkers(PowerModes mode)
        {
            var response = await _workerService.AllBackgroundWorkers_PowerModeAsync(mode);
            if (response == false)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        [HttpPost("induvidual-worker-powermode")]
        public async Task<IActionResult> StopBackgroundWorker(PowerModes mode, ControlWorker worker)
        {
            var response = await _workerService.IndividualWorker_PowerModeAsync(mode, worker);
            if (response == false)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        // USER CONTROL

        [HttpPost("lockout-customer")]
        public async Task<bool> LockoutCustomer(string email, DateTime endDate)
        {
            DateTime EndDate = new DateTime(2100, 1, 1);
            Nullable<DateTime> MyNullableDate;

            if (endDate == DateTime.MinValue)
                endDate = EndDate;

            var customer = await _customerManager.FindByEmailAsync(email);
            var lockUser = await _customerManager.SetLockoutEnabledAsync(customer, true);
            var lockDate = await _customerManager.SetLockoutEndDateAsync(customer, endDate);

            return lockDate.Succeeded && lockUser.Succeeded;
        }

        [HttpPost("unlock-customer")]
        public async Task<bool> UnlockCustomer(string email)
        {
            var customer = await _customerManager.FindByEmailAsync(email);
            var lockDisabled = await _customerManager.SetLockoutEnabledAsync(customer, false);
            var setLockoutEndDate = await _customerManager.SetLockoutEndDateAsync(customer, DateTime.Now);
            return lockDisabled.Succeeded && setLockoutEndDate.Succeeded;
        }

        [HttpPut("Update customer")]
        public async Task<IActionResult> UpdateCustomer(double money, Guid id)
        {
            var customer = await _service._genericCustomerRepository.GetUserEntityByIDAsync(id);
            customer.Balance = money;
            var response = await _service._genericCustomerRepository.UpdateUserEntityAsync(customer);
            if(!response)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        [HttpGet("Get-Customer")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var response =  await _service._genericCustomerRepository.GetUserEntityByIDAsync(id);
            if (response == null)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        [HttpGet("Get-Employee")]
        public async Task<ActionResult<Customer>> GetEmployee(Guid id)
        {
            var response = await _service._genericCustomerRepository.GetUserEntityByIDAsync(id);
            if (response == null)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        [HttpGet("Get-All-UserTypes")]
        public async Task<IActionResult> GetAllUserTypes()
        {
            var response = await _service.GetAllUserTypesAsync();
            if(response == null)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        [HttpGet("GetAllOrders")]
        public async Task<List<ProductOrder>> GetAllOrders()
        {
            return await _service.GetOrderdProductsAsync();
        }


    }
}
