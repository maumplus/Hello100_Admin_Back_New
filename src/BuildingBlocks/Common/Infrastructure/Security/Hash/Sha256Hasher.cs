using System.Security.Cryptography;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash
{
    public sealed class Sha256Hasher : IHasher
    {
        public string HashWithSalt(string plainText, string saltText)
        {
            if (plainText is null) 
                throw new ArgumentNullException(nameof(plainText));
            
            if (saltText is null) 
                throw new ArgumentNullException(nameof(saltText));

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText); // ASCII
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltText); // ASCII

            byte[] combined = new byte[plainBytes.Length + saltBytes.Length];

            plainBytes.CopyTo(combined, 0);
            saltBytes.CopyTo(combined.AsSpan(plainBytes.Length));

            byte[] hashBytes = SHA256.HashData(combined);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
