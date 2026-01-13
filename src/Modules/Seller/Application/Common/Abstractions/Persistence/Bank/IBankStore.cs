using Hello100Admin.Modules.Seller.Application.Features.Bank.ReadModels.GetBankList;

namespace Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Bank
{
    public interface IBankStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<List<GetBankListReadModel>> GetBankListAsync(CancellationToken cancellationToken = default);
    }
}
