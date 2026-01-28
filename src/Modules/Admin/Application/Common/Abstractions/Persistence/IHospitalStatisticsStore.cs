using Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IHospitalStatisticsStore
    {
        /// <summary>
        /// 접수구분통계 (접수방법별 접수 현황) 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="year"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<GetRegistrationStatsByMethodResult>> GetRegistrationStatsByMethodAsync(string hospNo, string year, CancellationToken token);

        /// <summary>
        /// 접수구분통계 (접수/취소 현황) 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="year"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<GetRegistrationStatusSummaryResult>> GetRegistrationStatusSummaryAsync(string hospNo, string year, CancellationToken token);

        /// <summary>
        /// 내원목적별 통계 (내원목적별 접수/취소 월별 통계) 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="yearMonth"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<GetRegistrationStatsByVisitPurposeResult>> GetRegistrationStatsByVisitPurposeAsync(string hospNo, string yearMonth, CancellationToken token);
    }
}
