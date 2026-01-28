using Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Hospital;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital
{
    public interface IHospitalStore
    {
        Task<GetHospitalModel?> GetHospital(string hospNo, CancellationToken cancellationToken = default);
        Task<List<GetHospMedicalTimeModel>> GetHospMedicalTimeList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetHospKeywordModel>> GetHospKeywordList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetHospitalMedicalModel>> GetHospitalMedicalList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetImageModel>> GetImageList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetHospMedicalTimeNewModel>> GetHospMedicalTimeNewList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetKeywordMasterModel>> GetKeywordMasterList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetDoctorListModel>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default);
    }
}
