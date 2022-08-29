

using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;

namespace P4Webshop.Interface.Generic
{

    /// <summary>
    /// Here is where i add the properties i want to use in the repository
    /// Id for getting id, and refreshtoken to get refreshtokens.
    /// </summary>
    public interface IUserEntities
    {
        public Guid Id { get; set; }

        //JWT
        public List<RefreshToken>? RefreshTokens { get; set; }
    }


    public interface IProductEntities
    {
        public Guid Id { get; set; }
        public ProductCategory Category { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public DateTime ProductCreated { get; set; }
    }
}
