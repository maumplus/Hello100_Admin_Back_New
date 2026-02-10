using Microsoft.Extensions.Logging;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash
{
    public interface IHasher
    {
        /// <summary>
        /// SHA-256 해시된 데이터 검증
        /// </summary>
        /// <param name="hashedText"></param>
        /// <param name="plainText"></param>
        /// <param name="salt"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public bool VerifyHashedData(string hashedText, string plainText, string salt, ILogger? logger);

        /// <summary>
        /// SHA-256 hashing with salt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="saltText"></param>
        /// <returns></returns>
        public string HashWithSalt(string plainText, string saltText);
    }
}
