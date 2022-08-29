
using P4Webshop.Interface.Generic;
using P4Webshop.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace P4Webshop.Models.Products
{
    [Table("OrderdProducts")]
    public class Product : BaseProduct, IProductEntities
    {
        public override Guid Id { get; set; }
        public override ProductCategory Category { get; set; }
        public virtual Guid ProductId { get; set; }
        public override string ProductName { get; set; }
        public override double ProductPrice { get; set; }
        public override int ProductQuantity { get; set; }

        //Empty constructor to initialize
        public Product() { }

        //Create product constructor
        public Product(ProductCategory category, Guid productid, string name, double price, int quantity)
        {
            Id = Guid.NewGuid();
            Category = category;
            ProductId = productid;
            ProductName = name;
            ProductPrice = price;
            ProductQuantity = quantity;
            ProductCreated = DateTime.UtcNow;
        }

        //Transfer from model 2 model
        public Product(Guid id, ProductCategory category, Guid productid, string name, double price, int quantity)
        {
            Id = id;
            Category = category;
            ProductId = productid;
            ProductName = name;
            ProductPrice = price;
            ProductQuantity = quantity;
        }
    }
}
