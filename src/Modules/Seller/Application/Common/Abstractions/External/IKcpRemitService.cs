using Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request;
using Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Result;

namespace Hello100Admin.Modules.Seller.Application.Common.Abstractions.External
{
    public interface IKcpRemitService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SellerRegisterResult?> RegisterSellerAsync(SellerRegisterRequest request);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountBalanceResult?> GetAccountBalanceAsync(AccountBalanceRequest request);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<RemitResult?> SendRemitAsync(RemitRequest request);
    }
}
