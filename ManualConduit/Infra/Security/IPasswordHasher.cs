namespace ManualConduit.Infra.Security
{
    public interface IPasswordHasher
    {
        byte[] Hash(string password, byte[] salt);
    }
}
