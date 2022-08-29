
using P4Webshop.Models.Products;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace P4Webshop.Models.Shopping
{
    [Table("ProductOrders")]
    public class ProductOrder
    {
        public ProductOrder() { }

        public Guid Id { get; set; }
        public DateTime OrderCreated { get; set; }
        public List<Product> OrderdProducts { get; set; }
        public Guid CustomerId { get; set; }
        [JsonIgnore]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public DeliveryAddress DeliveryAddress { get; set; }

        /// <summary>
        /// When converting to orders
        /// </summary>
        /// <param name="products"></param>
        /// <param name="customer"></param>
        public ProductOrder(List<Product> orderdProducts, Customer customer, DeliveryAddress deliveryAddress)
        {
            Id = Guid.NewGuid();
            OrderCreated = DateTime.Now;
            OrderdProducts = orderdProducts;
            CustomerId = customer.Id;
            Customer = customer;
            DeliveryAddress = deliveryAddress;
        }

    }
}
