using Microsoft.EntityFrameworkCore;
using P4Webshop.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace P4Webshop.Models.Shopping
{
    [Table("CustomerBasket")]
    public class CustomerBasket
    {
        public CustomerBasket() { }

        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public ProductCategory Category { get; set; }
        public double Price { get; set; }
        public Guid CustomerId { get; set; }
        [JsonIgnore]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public CustomerBasket(Guid productid, ProductCategory category, double price, Customer customer)
        {
            Id = Guid.NewGuid();
            ProductId = productid;
            Category = category;
            Price = price;
            Customer = customer;
        }

    }
}
