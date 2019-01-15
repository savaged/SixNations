using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SixNations.Server.Services
{
    public class EncryptionService : IEncryptionService
    {
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string _initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int _keysize = 256;
        
        private readonly string _passPhrase;

        public EncryptionService(string passPhrase)
        {
            _passPhrase = passPhrase;
        }

        //Encrypt
        public string Encrypt(string plainText)
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(_initVector);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var password = new PasswordDeriveBytes(_passPhrase, null);
            var keyBytes = password.GetBytes(_keysize / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
            ICryptoTransform encryptor = 
                symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            var value = string.Empty;
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = 
                    new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    var cipherTextBytes = memoryStream.ToArray();
                    memoryStream.Close();
                    cryptoStream.Close();
                    value = Convert.ToBase64String(cipherTextBytes);
                }
            }
            return value;
        }
    }
}
