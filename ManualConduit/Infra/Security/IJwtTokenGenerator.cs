using System.Threading.Tasks;

namespace ManualConduit.Infra.Security
{
    public interface IJwtTokenGenerator
    {
        Task<string> CreateToken(string username);
    }
}
