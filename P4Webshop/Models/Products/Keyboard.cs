
using P4Webshop.Interface.Generic;
using P4Webshop.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace P4Webshop.Models.Products
{
    [Table("Keyboard")]
    public class Keyboard : Product, IProductEntities
    {
        public override Guid Id { get; set; }
        public override ProductCategory Category { get; set; }
        public override string ProductName { get; set; }
        public override double ProductPrice { get; set; }
        public override int ProductQuantity { get; set; }
        public override DateTime ProductCreated { get; set; }

        //Empty constructor to initialize
        public Keyboard() { }

        //Create product constructor
        public Keyboard(ProductCategory category, string name, double price, int quantity)
        {
            Id = Guid.NewGuid();
            Category = category;
            ProductName = name;
            ProductPrice = price;
            ProductQuantity = quantity;
            ProductCreated = DateTime.UtcNow;
        }

        //Transfer from model 2 model
        public Keyboard(Guid id, ProductCategory category, string name, double price, int quantity)
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
