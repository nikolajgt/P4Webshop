

using P4Webshop.Interface.Generic;
using P4Webshop.Models;
using P4Webshop.Models.Base;
using P4Webshop.Models.DTO;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Interface
{
    public interface IAdminService
    {
        IGenericUserRepository<Customer> _genericCustomerRepository { get; set; }
        IGenericUserRepository<Admin> _genericAdminRepository { get; set; }
        IGenericUserRepository<Employee> _genericEmployeeRepository { get; set; }

        IGenericProductRepository<Keyboard> _genericKeyboardRepository { get; set; }
        IGenericProductRepository<Smartphone> _genericSmartphoneRepository { get; set; }
        IGenericProductRepository<Microphone> _genericMicrophoneRepository { get; set; }
        IGenericProductRepository<Mouse> _genericMouseRepository { get; set; }

        IShoppingRepository _shoppingRepository { get; set; }

        ///POST
        Task<bool> AddProductAsync(BaseProduct p);


        /// Get
        Task<User> GetUserByIDAsync(Roles role, Guid id);
        Task<AllUserTypesDTO> GetAllUserTypesAsync();
        Task<List<ProductOrder>> GetOrderdProductsAsync();
        Task<List<Customer>> GetAllCustomersAsync();
        bool IsHealthy();

        

        /// <summary>
        /// Unit of work
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();
        Task Dispose();


    }
}
