using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash
{
    public sealed class Sha256Hasher : IHasher
    {
        private readonly ILogger<Sha256Hasher> _logger;

        public Sha256Hasher(ILogger<Sha256Hasher> logger)
        {
            _logger = logger;
        }

        public bool VerifyHashedData(string hashedText, string plainText, string salt, ILogger? logger = null)
        {
            if (string.IsNullOrWhiteSpace(plainText) ||
                string.IsNullOrWhiteSpace(hashedText) ||
                string.IsNullOrWhiteSpace(salt))
            {
                (logger ?? _logger).LogWarning($"Invalid Data for hashing. Plain:{plainText}, Hashed:{hashedText}, Salt:{salt}", plainText, hashedText, salt);
                return false;
            }

            try
            {
                var computedHash = HashWithSalt(plainText, salt);

                // 타이밍 공격 방지를 위한 고정 시간 비교
                var isValid = CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(hashedText),
                    Encoding.ASCII.GetBytes(computedHash)
                );

                return isValid;
            }
            catch (Exception e)
            {
                (logger ?? _logger).LogError(e, $"Exception Occured while hashing. Plain:{plainText}, Hashed:{hashedText}, Salt:{salt}", plainText, hashedText, salt);
                return false;
            }
        }

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
