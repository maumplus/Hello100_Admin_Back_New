namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash
{
    public interface IHasher
    {
        /// <summary>
        /// SHA-256 hashing with salt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="saltText"></param>
        /// <returns></returns>
        public string HashWithSalt(string plainText, string saltText);
    }
}
