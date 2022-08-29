
using P4Webshop.Interface.Generic;
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace P4Webshop.Models
{
    [Table("Employee")]
    public class Employee : User, IUserEntities
    {
        public Employee() { }

        [Key]
        public override Guid Id { get; set; }
        public override string? UserName { get; set; }
        public override string? Email { get; set; }
        public override string? Firstname { get; set; }
        public override string? Lastname { get; set; }

        public override Roles Roles { get; set; }
        public EmployeeRoles EmployeeRole { get; set; }

        [JsonIgnore]
        public override List<RefreshToken>? RefreshTokens { get; set; }


        public Employee(string username, string email, string firstname, string lastname, Roles role)
        {
            Id = Guid.NewGuid();
            UserName = username;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Roles = role;
        }

        public Employee(Guid id, string username, string email, string firstname, string lastname, Roles role)
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
