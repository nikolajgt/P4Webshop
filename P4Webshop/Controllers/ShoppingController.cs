using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P4Webshop.Interface;
using P4Webshop.Models.Base;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;
using System.Security.Claims;

namespace P4Webshop.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {

        private readonly IShoppingService _service;
        private readonly IHttpContextAccessor _context;

        public ShoppingController(IShoppingService service, IHttpContextAccessor context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost("AddItemToBasket")]
        public async Task<IActionResult> AddItemsToBasket(Product product)
        {
            var id = GetUserFromHttp(out Roles role);
            var response = await _service.AddItemToBasketAsync(product, id, role);
            if (!response)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        [HttpGet("GetCustomerBasket")]
        public async Task<List<CustomerBasket>> GetCustomerbasketAsync()
        {
            var id = GetUserFromHttp(out Roles role);
            var response = await _service.GetCustomerBasketAsync(id, role);
            return response;
        }

        [HttpPost("BuyBasketItems")]
        public async Task<IActionResult> BuyBasketItems(DeliveryAddress address)
        {
            var id = GetUserFromHttp(out Roles role);
            var response = await _service.BuyItemsFromBasketAsync(id, address);
            if (!response)
                return new NotFoundObjectResult(response);

            return new OkObjectResult(response);
        }

        [HttpGet("Get user from http request")]
        private Guid GetUserFromHttp(out Roles roles)
        {
            try
            {
                var id = Guid.Parse(_context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value);
                roles = (Roles)Enum.Parse(typeof(Roles), _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);

                return id;
            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                roles = Roles.Customer;
                return Guid.Empty;
            }
        }
    }
}
