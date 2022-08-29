


using Microsoft.EntityFrameworkCore;
using P4Webshop.Models.Products;
using P4Webshop.Models.Shopping;

namespace P4Webshop.Models
{
    public class MyDbContext : DbContext
    {
        
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        //User types
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        //Products
        public DbSet<Keyboard> Keyboard { get; set; }
        public DbSet<Smartphone> Smartphone { get; set; }
        public DbSet<Microphone> Microphone { get; set; }
        public DbSet<Mouse> Mouse { get; set; }

        //Orders
        public DbSet<CustomerBasket> Basket { get; set; }
        public DbSet<ProductOrder> Orders { get; set; }
        public DbSet<DeliveryModel> Delivery { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                entityType.BaseType = null;
        }
       

    }
}
