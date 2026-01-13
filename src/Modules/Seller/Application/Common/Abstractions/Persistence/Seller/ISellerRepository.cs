using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSeller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSellerRemit;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemit;

namespace Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISellerRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        public Task<long> InsertTbHospSellerAsync(CreateSellerCommand req, string sellerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> UpdateTbHospSellerIsSyncByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<long> InsertTbHospSellerRemitAsync(CreateSellerRemitCommand req, CancellationToken cancellationToken = default);

        /// <summary>
        /// 병원 판매자 계좌 정보 수정
        /// </summary>
        /// <param name="param"></param>
        /// <param name="id"></param>
        /// <param name="etc"></param>
        /// <returns></returns>
        public Task<int> UpdateSellerRemitAsync(UpdateSellerRemitParams param, int id, string? etc, CancellationToken cancellationToken = default);

        /// <summary>
        /// 병원 판매자 계좌 정보 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <param name="etc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> DeleteSellerRemitAsync(int id, string? etc, CancellationToken cancellationToken = default);

        /// <summary>
        /// 병원 판매자 계좌 활성화/비활성화 처리
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> UpdateSellerRemitEnabledAsync(int id, bool enabled, CancellationToken cancellationToken = default);
    }
}
