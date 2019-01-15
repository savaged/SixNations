namespace SixNations.Server.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
    }
}