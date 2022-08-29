using System.ComponentModel.DataAnnotations.Schema;

namespace P4Webshop.Models.Shopping
{
    [Table("DeliveryOrders")]
    public class DeliveryModel
    {
        public Guid Id { get; set; }
        public ProductOrder Order { get; set; }
        public DateTime DeliveryStarted { get; set; }
        public DateTime? DeliveryEnded { get; set; }
        public bool DeliveryBegun { get; set; }
        public string? Address { get; set; }
        public int? AddressNumber { get; set; }
        public string? City { get; set; }
        public int? Zipcode { get; set; }

        public DeliveryModel(ProductOrder order, string address, int addressNumber, string city, int zipcode)
        {
            Id = Guid.NewGuid();
            Order = order;
            DeliveryBegun = false;
            Address = address;
            AddressNumber = addressNumber;
            City = city;
            Zipcode = zipcode;
        }

        public DeliveryModel()
        {
            DeliveryStarted = DateTime.Now;
            DeliveryBegun = true;
            DeliveryEnded = DeliveryStarted.AddDays(3);
        }

        public DeliveryModel(DateTime ended)
        {
            DeliveryEnded = ended;
        }
    }
}
