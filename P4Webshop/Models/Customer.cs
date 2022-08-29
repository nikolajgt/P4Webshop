
using P4Webshop.Interface.Generic;
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;
using P4Webshop.Models.Shopping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace P4Webshop.Models
{
    [Table("Customer")]
    public class Customer : User, IUserEntities
    {
        public Customer() { }

        [Key]
        public override Guid Id { get; set; }
        public override string? UserName { get; set; }
        public override string? Email { get; set; }
        public override string? Firstname { get; set; }
        public override string? Lastname { get; set; }
        public override Roles Roles { get; set; }
        public double? Balance { get; set; }
        public DeliveryAddress? PrimaryAddress { get; set; }
        public List<DeliveryAddress>? ShippingAddresses { get; set; }

        [JsonIgnore]
        public override List<RefreshToken>? RefreshTokens { get; set; }

        public virtual List<CustomerBasket> CustomerBaskets { get; set; }


        /// <summary>
        /// Contructor for creating normal user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="balance"></param>
        /// <param name="role"></param>
        public Customer(string username, string email, string firstname, string lastname, double balance, Roles role)
        {
            Id = Guid.NewGuid();
            UserName = username;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Balance = balance;
            Roles = role;
            CustomerBaskets = new List<CustomerBasket>();
            RefreshTokens = new List<RefreshToken>();
            PrimaryAddress = new DeliveryAddress();
            ShippingAddresses = new List<DeliveryAddress>();
        }

        public Customer(Guid id, string username, string email, string firstname, string lastname, double balance, Roles role)
        {
            Id = id;
            UserName = username;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Balance = balance;
            Roles = role;
        }
    }
}

