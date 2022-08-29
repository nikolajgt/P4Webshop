using Microsoft.EntityFrameworkCore;
using P4Webshop.Interface;
using P4Webshop.Models;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Repository
{
    public class ShoppingRepository : IShoppingRepository
    {
        private readonly MyDbContext _db;
        private readonly ILogger _logger;

        public ShoppingRepository(MyDbContext db, ILoggerFactory loggerFactory)
        {
            _db = db;
            _logger = loggerFactory.CreateLogger<ShoppingRepository>();
        }

        public async Task<Customer> GetCustomerAsync(Guid id)
        {
            try
            {
                var response = await _db.Customers
                                        .Include(x => x.CustomerBaskets)
                                        .Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $" Error at Shopping Repository GetCustomerBasketAsync");
                return null;
            }
        }

        public async Task<bool> UpdateCustomerBasketAsync(Customer customer)
        {
            try
            {
                var response = _db.Customers.Update(customer);
                return (await _db.SaveChangesAsync()) > 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $" Error at Shopping Repository UpdateCustomerBasketAsync");
                return false;
            }
        }

        public async Task<bool> AddToCustomerBasketAsync(CustomerBasket basket)
        {
            try
            {
                await _db.Basket.AddAsync(basket);
                return (await _db.SaveChangesAsync()) > 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $" Error at Shopping Repository AddToCustomerBasketAsync");
                return false;
            }
        }

        public async Task<bool> AddBasketToOrdersTableAsync(ProductOrder order)
        {
            try
            {
                await _db.Orders.AddAsync(order);
                
                return (await _db.SaveChangesAsync()) > 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $" Error at Shopping Repository AddBasketToOrdersTableAsync");
                return false;
            }
        }

        public async Task<List<ProductOrder>> GetOrderdProductsAsync()
        {
            try
            {
                return await _db.Orders
                    .Include(x => x.OrderdProducts)
                    .Include(x => x.DeliveryAddress)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $" Error at Shopping Repository AddBasketToOrdersTableAsync");
                return null;
            }
        }



    }
}
