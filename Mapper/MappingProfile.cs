using Api.Models;
using AutoMapper;

namespace Api.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, DTOUser>().ForMember(_destiny => _destiny.Roles, _source => _source.MapFrom(_mapped => _mapped.UserRoles.Select(x => x.Role.Name).ToList()));
        CreateMap<Role, DTORole>();
    }
}