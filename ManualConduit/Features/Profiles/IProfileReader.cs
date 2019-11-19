using System.Threading.Tasks;

namespace ManualConduit.Features.Profiles
{
    public interface IProfileReader
    {
        Task<ProfileEnvelope> ReadProfile(string username);
    }
}
