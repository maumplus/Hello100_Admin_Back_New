using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account
{
    public interface IAccountRepository
    {
        Task<TbEghisHospInfoEntity?> GetEghisHospInfoAsync(DbSession db, string HospNo, CancellationToken cancellationToken);
        Task<int> UpdateAdminAsync(DbSession db, TbAdminEntity entity, CancellationToken cancellationToken);
        Task<int> UpdateEghisHospInfoAsync(DbSession db, TbEghisHospInfoEntity entity, CancellationToken cancellationToken);
        Task<int> UpdateEghisHospInfoAsync(DbSession db, TbEghisHospQrInfoEntity entity, CancellationToken cancellationToken);
        Task<int> UpdateEghisHospInfoAsync(DbSession db, TbEghisRecertDocInfoEntity entity, CancellationToken cancellationToken);
        Task<int> InsertEghisHospVisitPurposeInfoAsync(DbSession db, List<TbEghisHospVisitPurposeInfoEntity> entities, CancellationToken cancellationToken);
        Task<int> InsertEghisHospSettingsInfoAsync(DbSession db, TbEghisHospSettingsInfoEntity entity, CancellationToken cancellationToken);
        Task<int> InsertTbEghisHospMedicalTimeNewAsync(DbSession db, List<TbEghisHospMedicalTimeNewEntity> entities, CancellationToken cancellationToken);
    }
}
