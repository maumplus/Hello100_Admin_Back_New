using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposeDetail;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetCertificates;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetVisitPurposeDetail;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetVisitPurposes;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose
{
    public interface IVisitPurposeStore
    {
        /// <summary>
        /// 내원 목적 관리 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetVisitPurposesReadModel> GetVisitPurposesAsync(string hospKey, CancellationToken token);

        /// <summary>
        /// 내원 목적 관리 > 내원 목적 편집 조회
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetVisitPurposeDetailReadModel> GetVisitPurposeDetailAsync(GetVisitPurposeDetailQuery req, CancellationToken token);

        /// <summary>
        /// 내원 목적 관리 > 제증명문서관리 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetCertificatesReadModel> GetCertificatesAsync(string hospKey, CancellationToken token);

        /// <summary>
        /// 문진표 목록 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ListResult<GetQuestionnairesResult>> GetQuestionnairesAsync(DbSession db, string hospNo, CancellationToken ct);
    }
}
