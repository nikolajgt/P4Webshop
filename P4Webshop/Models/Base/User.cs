using Microsoft.AspNetCore.Identity;
using P4Webshop.Models.JWT;
using System.Text.Json.Serialization;


namespace P4Webshop.Models.Base
{
    public class User : IdentityUser<Guid>
    {
        public User() { }
        /// <summary>
        /// This is our base User model, all user entities is inherited from this. This class inherits from IdentityUser 
        /// and is a part of Identity Core
        /// </summary>

        public override Guid Id { get; set; }
        public override string? UserName { get; set; }                 
        public override string? Email { get; set; }
        public virtual string? Firstname { get; set; }
        public virtual string? Lastname { get; set; }
        public virtual Roles Roles { get; set; }

        [JsonIgnore]
        public virtual List<RefreshToken>? RefreshTokens { get; set; }


        public User(string username, string email, string firstname, string lastname, Roles roles)
        {
            Id = Guid.NewGuid();
            UserName = username;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Roles = roles;
        }
    }
}
