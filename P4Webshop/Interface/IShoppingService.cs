
using P4Webshop.Interface.Generic;
using P4Webshop.Models.Base;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Interface
{
    public interface IShoppingService
    {

        IGenericProductRepository<Keyboard> _genericKeyboardRepository { get; set; }
        IGenericProductRepository<Smartphone> _genericSmartphoneRepository { get; set; }
        IGenericProductRepository<Microphone> _genericMicrophoneRepository { get; set; }
        IGenericProductRepository<Mouse> _genericMouseRepository { get; set; }

        Task<bool> AddItemToBasketAsync(Product product, Guid id, Roles role);
        Task<bool> BuyItemsFromBasketAsync(Guid customerid, DeliveryAddress address);
        Task<List<CustomerBasket>> GetCustomerBasketAsync(Guid id, Roles role);
        Task<bool> ChangeCustomerPrimaryAddressAsync(DeliveryAddress address);
    }
}
