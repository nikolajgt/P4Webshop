
using P4Webshop.Interface.Generic;
using P4Webshop.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace P4Webshop.Models.Products
{
    [Table("Smartphone")]
    public class Smartphone : Product, IProductEntities
    {
        public override Guid Id { get; set; }
        public override ProductCategory Category { get; set; }
        public override string ProductName { get; set; }
        public override double ProductPrice { get; set; }
        public override int ProductQuantity { get; set; }
        public override DateTime ProductCreated { get; set; }

        //Empty constructor to initialize
        public Smartphone() { }

        //Create product constructor
        public Smartphone(ProductCategory category, string name, double price, int quantity)
        {
            Id = Guid.NewGuid();
            Category = category;
            ProductName = name;
            ProductPrice = price;
            ProductQuantity = quantity;
            ProductCreated = DateTime.UtcNow;
        }

        //Transfer from model 2 model
        public Smartphone(Guid id, ProductCategory category, string name, double price, int quantity)
        {
            Id = id;
            Category = category;
            ProductName = name;
            ProductPrice = price;
            ProductQuantity = quantity;
            ProductCreated = DateTime.UtcNow;
        }
    }
}
