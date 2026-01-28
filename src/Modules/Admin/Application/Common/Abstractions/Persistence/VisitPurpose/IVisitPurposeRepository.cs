using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateCertificates;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateVisitPurposes;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.CreateVisitPurpose;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNonNhisHealthScreening;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose
{
    public interface IVisitPurposeRepository
    {
        /// <summary>
        /// 내원 목적 목록 편집
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> BulkUpdateVisitPurposesAsync(BulkUpdateVisitPurposesCommand req, CancellationToken cancellationToken);

        /// <summary>
        /// 내원 목적 승인요청 생성
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>승인 요청 ID</returns>
        public Task<int> CreateVisitPurposeApprovalAsync(DbSession db, string hospKey, string apprType, string data, string reqAId, CancellationToken cancellationToken);

        /// <summary>
        /// 신규 이지스병원내원목적정보(제목, 상세항목) 추가 및 승인요청정보 갱신
        /// </summary>
        /// <param name="req"></param>
        /// <param name="apprId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> CreateVisitPurposeAsync(DbSession db, CreateVisitPurposeCommand req, int apprId, CancellationToken cancellationToken);


        /// <summary>
        /// 국민건강보험공단 건강검진 내원목적 설정 업데이트
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="showYn"></param>
        /// <param name="role"></param>
        /// <param name="detailShowYn"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> UpdateVisitPurposeForNhisHealthScreeningAsync(string hospKey, string showYn, int role, List<string>? detailShowYn, CancellationToken cancellationToken);

        /// <summary>
        /// 국민건강보험공단 건강검진 외 나머지 내원목적 설정 업데이트
        /// </summary>
        /// <param name="req"></param>
        /// <param name="apprId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> UpdateVisitPurposeForNonNhisHealthScreeningAsync(DbSession db, UpdateVisitPurposeForNonNhisHealthScreeningCommand req, int apprId, CancellationToken cancellationToken);


        /// <summary>
        /// 내원 목적 삭제 (공단 검진은 삭제 불가)
        /// </summary>
        /// <param name="vpCd"></param>
        /// <param name="hospKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> DeleteVisitPurposeAsync(string vpCd, string hospKey, CancellationToken cancellationToken);

        /// <summary>
        /// 제증명문서관리 일괄 수정
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> BulkUpdateCertificatesAsync(BulkUpdateCertificatesCommand req, CancellationToken cancellationToken);
    }
}
