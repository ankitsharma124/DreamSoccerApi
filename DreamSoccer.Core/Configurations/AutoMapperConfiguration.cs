using AutoMapper;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
