using System.ComponentModel.DataAnnotations.Schema;

namespace P4Webshop.Models.Shopping
{
    [Table("Addresses")]
    public class DeliveryAddress
    {
        public Guid id { get; set; }
        public string? Address { get; set; }
        public int? AddressNumber { get; set; }
        public string? City { get; set; }
        public int? Zipcode { get; set; }

        public DeliveryAddress() { }
        public DeliveryAddress(string? address, int? addressNumber, string? city, int? zipcode)
        {
            id = Guid.NewGuid();
            Address = address;
            AddressNumber = addressNumber;
            City = city;
            Zipcode = zipcode;
        }
    }
}
