using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Departments.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence
{
    public interface IDepartmentsStore
    {
        /// <summary>
        /// 전체 진료과목 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="clsCd"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListResult<GetDepartmentsResult>> GetDepartmentsAsync(DbSession db, string clsCd, CancellationToken cancellationToken = default);
    }
}
