

using P4Webshop.Extensions.ModelConverter;
using P4Webshop.Interface;
using P4Webshop.Interface.Generic;
using P4Webshop.Models;
using P4Webshop.Models.Base;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;
using P4Webshop.Repository;
using P4Webshop.Repositorys;

namespace P4Webshop.Services
{
    public class ShoppingService : IShoppingService
    {
        private readonly IShoppingRepository _shoppingRepository;
        private readonly ILogger _logger;


        public IGenericProductRepository<Keyboard> _genericKeyboardRepository { get; set; }
        public IGenericProductRepository<Smartphone> _genericSmartphoneRepository { get; set; }
        public IGenericProductRepository<Microphone> _genericMicrophoneRepository { get; set; }
        public IGenericProductRepository<Mouse> _genericMouseRepository { get; set; }

        public ShoppingService(ILoggerFactory loggerFactory, IShoppingRepository shoppingRepository, MyDbContext context)
        {
            _logger = loggerFactory.CreateLogger("ShoppingService");
            _shoppingRepository = shoppingRepository;
            _genericKeyboardRepository = new GenericProductRepository<Keyboard>(context, _logger);
            _genericSmartphoneRepository = new GenericProductRepository<Smartphone>(context, _logger);
            _genericMicrophoneRepository = new GenericProductRepository<Microphone>(context, _logger);
            _genericMouseRepository = new GenericProductRepository<Mouse>(context, _logger);
        }

        public async Task<bool> AddItemToBasketAsync(Product product, Guid id, Roles role)
        {
            try
            {
                var customer = await _shoppingRepository.GetCustomerAsync(id);
                if (customer == null)
                    return false;

                switch (product.Category)
                {
                    case ProductCategory.Keyboard:
                        var keyboard = await _genericKeyboardRepository.GetProductEntityByIDAsync(product.ProductId);
                        if (keyboard == null)
                            return false;

                        var basketKey = new CustomerBasket(keyboard.Id, keyboard.Category, keyboard.ProductPrice, customer);
                        return await _shoppingRepository.AddToCustomerBasketAsync(basketKey);

                    case ProductCategory.Smartphone:
                        var smartphone = await _genericSmartphoneRepository.GetProductEntityByIDAsync(product.ProductId);
                        if (smartphone == null)
                            return false;

                        var basketSmart = new CustomerBasket(smartphone.Id, smartphone.Category, smartphone.ProductPrice, customer);
                        return await _shoppingRepository.AddToCustomerBasketAsync(basketSmart);

                    case ProductCategory.Microphone:
                        var microphone = await _genericMicrophoneRepository.GetProductEntityByIDAsync(product.ProductId);
                        if (microphone == null)
                            return false;

                        var basketMicro = new CustomerBasket(microphone.Id, microphone.Category, microphone.ProductPrice, customer);
                        return await _shoppingRepository.AddToCustomerBasketAsync(basketMicro);

                    case ProductCategory.Mouse:
                        var mouse = await _genericMouseRepository.GetProductEntityByIDAsync(product.ProductId);
                        if (mouse == null)
                            return false;

                        var basketMouse = new CustomerBasket(mouse.Id, mouse.Category, mouse.ProductPrice, customer);
                        return await _shoppingRepository.AddToCustomerBasketAsync(basketMouse);
                }
                return false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
           
        }

        public async Task<List<CustomerBasket>> GetCustomerBasketAsync(Guid id, Roles role)
        {
            var response = await _shoppingRepository.GetCustomerAsync(id);
            return response.CustomerBaskets;
        }

        public async Task<bool> BuyItemsFromBasketAsync(Guid customerid, DeliveryAddress address)
        {
            try
            {
                List<Product> orders = new List<Product>();
                Customer customer = await _shoppingRepository.GetCustomerAsync(customerid);

                if (customer.CustomerBaskets == null)
                    return false;

                double priceForAllItems = 0;

                foreach (var p in customer.CustomerBaskets)
                {
                    switch (p.Category)
                    {
                        case ProductCategory.Keyboard:
                            var keyboard = await _genericKeyboardRepository.GetProductEntityByIDAsync(p.ProductId);
                            if (keyboard == null)
                                break;

                            keyboard.ProductQuantity -= 1;

                            var newKey = GenericModelConverters.CreateProductGeneric<Keyboard>(keyboard);
                            orders.Add(newKey);

                            await _genericKeyboardRepository.UpdateProductEntityAsync(keyboard);
                            break;

                        case ProductCategory.Smartphone:
                            var smartphone = await _genericSmartphoneRepository.GetProductEntityByIDAsync(p.ProductId);
                            if (smartphone == null)
                                return false;


                            var newSmart = GenericModelConverters.CreateProductGeneric<Smartphone>(smartphone);
                            orders.Add(newSmart);

                            smartphone.ProductQuantity -= 1;

                            await _genericSmartphoneRepository.UpdateProductEntityAsync(smartphone);
                            break;

                        case ProductCategory.Microphone:
                            var microphone = await _genericMicrophoneRepository.GetProductEntityByIDAsync(p.ProductId);
                            if (microphone == null)
                                return false;

                            var newMicro = GenericModelConverters.CreateProductGeneric<Microphone>(microphone);
                            orders.Add(newMicro);

                            microphone.ProductQuantity -= 1;

                            await _genericMicrophoneRepository.UpdateProductEntityAsync(microphone);
                            break;

                        case ProductCategory.Mouse:
                            var mouse = await _genericMouseRepository.GetProductEntityByIDAsync(p.ProductId);
                            if (mouse == null)
                                return false;

                            var newMouse = GenericModelConverters.CreateProductGeneric<Mouse>(mouse);
                            orders.Add(newMouse);

                            mouse.ProductQuantity -= 1;

                            await _genericMouseRepository.UpdateProductEntityAsync(mouse);
                            break;
                    }

                    priceForAllItems += p.Price;
                }

                if (priceForAllItems > customer.Balance)
                    return false;

                customer.Balance -= priceForAllItems;
                customer.CustomerBaskets = new List<CustomerBasket>();

                var transactionCompleted = await _shoppingRepository.UpdateCustomerBasketAsync(customer);
                if (!transactionCompleted)
                    return false;

                ProductOrder order = new ProductOrder(orders, customer, address);
                return await _shoppingRepository.AddBasketToOrdersTableAsync(order);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> ChangeCustomerPrimaryAddressAsync(DeliveryAddress address)
        {
            throw new NotImplementedException();
        }

    }
}
