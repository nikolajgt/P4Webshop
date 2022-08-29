using AutoMapper;
using P4Webshop.Models;
using P4Webshop.Models.Base;

namespace P4Webshop.Extensions.ModelConverter
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, Customer>();
            CreateMap<User, Admin>();
            CreateMap<User, Employee>();
        }
    }
}
