
using P4Webshop.Interface.Generic;
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace P4Webshop.Models
{
    [Table("Admin")]
    public class Admin : User, IUserEntities
    {
        public Admin() { }

        [Key]
        public override Guid Id { get; set; }
        public override string? UserName { get; set; }                 //Commented becuase of inheretance, could also override it
        public override string? Email { get; set; }
        public override string? Firstname { get; set; }
        public override string? Lastname { get; set; }
        public override Roles Roles { get; set; }

        public override List<RefreshToken>? RefreshTokens { get; set; }

        public Admin(string username)
        {
            UserName = username;
        }

        //Creates new user
        public Admin(string username, string email, string firstname, string lastname, Roles role)
        {
            Id = Guid.NewGuid();
            UserName = username;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Roles = role;
        }

        public Admin(Guid id, string username, string email, string firstname, string lastname, Roles role)
        {
            Id = id;
            UserName = username;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Roles = role;
        }
    }
}
