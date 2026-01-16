using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalHistory;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.GetUntactMedicalHistory;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage
{
    public interface IServiceUsageStore
    {
        /// <summary>
        /// 비대면 진료 내역 리스트 조회
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetUntactMedicalHistoryReadModel?> GetUntactMedicalHistoryAsync(GetUntactMedicalHistoryQuery req, CancellationToken token);
    }
}
