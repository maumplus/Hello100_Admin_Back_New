using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospital
{
    public class GetHospitalQuery : IRequest<Result<GetHospitalResponse>>
    {
        public string HospNo { get; set; } = string.Empty;
    }
}
