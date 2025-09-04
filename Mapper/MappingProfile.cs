using Api.Models;
using AutoMapper;

namespace Api.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, DTOUser>().ForMember(_destiny => _destiny.Role, _source => _source.MapFrom(_mapped => _mapped.UserRoles.Any() ? _mapped.UserRoles.First().Role : null));
        CreateMap<Role, DTORole>();
    }
}