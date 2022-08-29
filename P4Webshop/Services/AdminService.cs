

using P4Webshop.Interface;
using P4Webshop.Interface.Generic;
using P4Webshop.Models;
using P4Webshop.Models.Base;
using P4Webshop.Models.DTO;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;
using P4Webshop.Repository;
using P4Webshop.Repositorys;

namespace P4Webshop.Services
{
    public class AdminService : IAdminService
    {
        /// <summary>
        /// USERS
        /// </summary>
        public IGenericUserRepository<Customer> _genericCustomerRepository { get; set; }
        public IGenericUserRepository<Admin> _genericAdminRepository { get; set; }
        public IGenericUserRepository<Employee> _genericEmployeeRepository { get; set; }

        /// <summary>
        /// Products
        /// </summary>
        public IGenericProductRepository<Keyboard> _genericKeyboardRepository { get; set; }
        public IGenericProductRepository<Smartphone> _genericSmartphoneRepository { get; set; }
        public IGenericProductRepository<Microphone> _genericMicrophoneRepository { get; set; }
        public IGenericProductRepository<Mouse> _genericMouseRepository { get; set; }
        public IShoppingRepository _shoppingRepository { get; set; }

        private readonly MyDbContext _context;
        private readonly ILogger _logger;
        public AdminService(MyDbContext context, IShoppingRepository shoppingRepository , ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("AdminService");
            _shoppingRepository = shoppingRepository;
            _genericCustomerRepository = new GenericUserRepository<Customer>(context, _logger);
            _genericAdminRepository = new GenericUserRepository<Admin>(context, _logger);
            _genericEmployeeRepository = new GenericUserRepository<Employee>(context, _logger);
            _genericKeyboardRepository = new GenericProductRepository<Keyboard>(context, _logger);
            _genericSmartphoneRepository = new GenericProductRepository<Smartphone>(context, _logger);
            _genericMicrophoneRepository = new GenericProductRepository<Microphone>(context, _logger);
            _genericMouseRepository = new GenericProductRepository<Mouse>(context, _logger);
        }

        public async Task<bool> AddProductAsync(BaseProduct p)
        {
            bool addProduct = false;
            switch (p.Category)
            {
                case ProductCategory.Keyboard:
                    Keyboard key = new Keyboard(p.Category, p.ProductName, p.ProductPrice, p.ProductQuantity);
                    addProduct =await _genericKeyboardRepository.PostProductEntityAsync(key);
                    break;

                case ProductCategory.Smartphone:
                    Smartphone smart = new Smartphone(p.Category, p.ProductName, p.ProductPrice, p.ProductQuantity);
                    addProduct = await _genericSmartphoneRepository.PostProductEntityAsync(smart);
                    break;

                case ProductCategory.Microphone:
                    Microphone micro = new Microphone(p.Category, p.ProductName, p.ProductPrice, p.ProductQuantity);
                    addProduct = await _genericMicrophoneRepository.PostProductEntityAsync(micro);
                    break;

                case ProductCategory.Mouse:
                    Mouse mouse = new Mouse(p.Category, p.ProductName, p.ProductPrice, p.ProductQuantity);
                    addProduct = await _genericMouseRepository.PostProductEntityAsync(mouse);
                    break;
            }
            await SaveAsync();
            return addProduct;
        }

        public async Task<User> GetUserByIDAsync(Roles role, Guid id)
        {
            try
            {
                switch(role)
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

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _genericCustomerRepository.GetAllUserEntitiesAsync();
        }

        public async Task<AllUserTypesDTO> GetAllUserTypesAsync()
        {
            var customers = await _genericCustomerRepository.GetAllUserEntitiesAsync();
            var admins = await _genericAdminRepository.GetAllUserEntitiesAsync();
            var employees = await _genericEmployeeRepository.GetAllUserEntitiesAsync();
            return new AllUserTypesDTO(customers, admins, employees);
        }

        public async Task<List<ProductOrder>> GetOrderdProductsAsync()
        {
            return await _shoppingRepository.GetOrderdProductsAsync();
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
