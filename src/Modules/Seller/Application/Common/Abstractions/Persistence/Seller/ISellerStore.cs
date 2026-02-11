using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.CreateSeller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerDetail;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerRemitWaitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.UpdateSellerRemit;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Results;
using Hello100Admin.Modules.Seller.Domain.Entities;

namespace Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller
{
    /// <summary>
    /// Read
    /// </summary>
    public interface ISellerStore
    {
        /// <summary>
        /// 비대면 진료 신청 상태가 [승인] 상태인 병원 정보 조회
        /// </summary>
        /// <param name="hospNo">요양 기관 번호</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns></returns>
        public Task<GetApprovedUntactHospitalInfoReadModel?> GetApprovedUntactHospitalInfoAsync(string hospNo, CancellationToken cancellationToken = default);

        /// <summary>
        /// 병원 판매자 등록 계좌 수 조회
        /// </summary>
        /// <param name="hospNo">요양 기관 번호</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns></returns>
        public Task<long> GetHospitalSellerCountAsync(string hospNo, CancellationToken cancellationToken = default);

        /// <summary>
        /// 병원 판매자 계좌 정보 리스트 조회
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<List<GetHospSellerListReadModel>> GetHospSellerListAsync(GetSellerListQuery req, CancellationToken cancellationToken = default);

        /// <summary>
        /// 일련 번호(고유 ID)로 병원 판매자 계좌 정보 조회
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GetHospSellerDetailInfoReadModel?> GetHospSellerDetailInfoAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 일련 번호(고유 ID)를 통한 송금 상태에 따른 금액 및 카운트 조회
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GetSellerRemitCountReadModel?> GetSellerRemitCountAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 일련 번호(고유 ID)로 병원 판매자 계좌 정보 조회 (송금 정보 수정용)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GetHospSellerRemitWaitInfoReadModel?> GetHospSellerRemitWaitInfoAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 병원 판매자 송금 내역 리스트 조회
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<List<GetHospSellerRemitListReadModel>> GetHospSellerRemitListAsync(GetSellerRemitListQuery req, CancellationToken cancellationToken = default);

        /// <summary>
        /// 판매자 송금 대기 내역 조회
        /// </summary>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<List<GetSellerRemitWaitListReadModel>> GetSellerRemitWaitListAsync(string startDt, string endDt, CancellationToken cancellationToken = default);

        /// <summary>
        /// 병원 검색
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchText"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<List<GetHospitalsResult>> GetHospitalsAsync(DbSession db, string searchText, CancellationToken ct = default);

        /// <summary>
        /// 관리자 아이디로 관리자 정보 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="aId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<TbAdminEntity?> GetAdminByAIdAsync(DbSession db, string aId, CancellationToken ct = default);
    }
}
