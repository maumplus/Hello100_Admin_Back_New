using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface IRequestsManagementRepository
    {
        public Task<int> UpdateRequestBugAsync(DbSession db, TbHospitalProposalInfoEntity entity, CancellationToken ct);
        
        public Task<int> UpdateRequestUntactAsync(DbSession db, TbEghisDoctUntactJoinEntity entity, CancellationToken ct);
    }
}
