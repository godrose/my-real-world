using AutoMapper;
using ManualConduit.Domain;

namespace ManualConduit.Features.Profiles
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, Profile>(MemberList.None);
        }
    }
}