
using P4Webshop.Models;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Interface
{
    public interface IShoppingRepository
    {
        Task<Customer> GetCustomerAsync(Guid id);
        Task<bool> UpdateCustomerBasketAsync(Customer customer);
        Task<bool> AddToCustomerBasketAsync(CustomerBasket basket);
        Task<bool> AddBasketToOrdersTableAsync(ProductOrder order);
        Task<List<ProductOrder>> GetOrderdProductsAsync();
    }
}
