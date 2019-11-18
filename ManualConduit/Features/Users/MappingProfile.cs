using AutoMapper;
using ManualConduit.Domain;

namespace ManualConduit.Features.Users
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, User>(MemberList.None);
        }
    }
}