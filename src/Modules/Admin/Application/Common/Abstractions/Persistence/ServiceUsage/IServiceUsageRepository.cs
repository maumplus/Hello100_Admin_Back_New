using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage
{
    public interface IServiceUsageRepository
    {
        /// <summary>
        /// 알림톡 신청 내역 등록
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> SubmitAlimtalkApplicationAsync(SubmitAlimtalkApplicationCommand req, CancellationToken cancellationToken = default);

        /// <summary>
        /// 알림톡 신청 내역 삭제
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospKey"></param>
        /// <param name="tmpType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> DeleteAlimtalkApplicationAsync(DbSession db, string hospNo, string hospKey, string tmpType, CancellationToken cancellationToken);
    }
}
