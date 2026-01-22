using Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels;
using Hello100Admin.Modules.Admin.Domain.Entities;
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
        Task<List<GetDoctorListModel>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default);
    }
}
