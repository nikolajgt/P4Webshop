using System.ComponentModel.DataAnnotations.Schema;

namespace P4Webshop.Models.Base
{
    public class BaseProduct
    {
        public virtual Guid Id { get; set; }
        public virtual ProductCategory Category { get; set; }
        public virtual string ProductName { get; set; }
        public virtual double ProductPrice { get; set; }
        public virtual int ProductQuantity { get; set; }
        public virtual DateTime ProductCreated { get; set; }

        //Empty constructor to initialize
        public BaseProduct() { }

        //Create product constructor
        public BaseProduct(ProductCategory category, string name, double price, int quantity)
        {
            Id = Guid.NewGuid();
            Category = category;
            ProductName = name;
            ProductPrice = price;
            ProductQuantity = quantity;
            ProductCreated = DateTime.UtcNow;
        }

        //Transfer from model 2 model
        public BaseProduct(Guid id, ProductCategory category, string name, double price, int quantity)
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
