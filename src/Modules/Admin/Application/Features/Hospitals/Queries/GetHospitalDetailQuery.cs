using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Queries
{
    public record GetHospitalDetailQuery(string HospKey) : IQuery<Result<GetHospitalDetailResult>>;

    public class GetHospitalDetailQueryValidator : AbstractValidator<GetHospitalDetailQuery>
    {
        public GetHospitalDetailQueryValidator() 
        {

        }
    }

    public class GetHospitalDetailQueryHandler : IRequestHandler<GetHospitalDetailQuery, Result<GetHospitalDetailResult>>
    {
        private readonly ILogger<GetHospitalDetailQueryHandler> _logger;
        private readonly IHospitalsStore _hospitalsStore;
        private readonly IDbSessionRunner _db;

        public GetHospitalDetailQueryHandler(ILogger<GetHospitalDetailQueryHandler> logger, IHospitalsStore hospitalsStore, IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalsStore = hospitalsStore;
            _db = db;
        }

        public async Task<Result<GetHospitalDetailResult>> Handle(GetHospitalDetailQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle SearchHospitalsQueryHandler");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalsStore.GetHospitalDetailAsync(session, req.HospKey, token),
            ct);

            if (result.DeptCd != null)
            {
                var arrDeptCode = result.DeptCd?.Split(',').ToArray();
                var arrDeptName = result.DeptName?.Split(',').ToArray();
                result.DeptCodes = new List<GetHospitalDetailResultDeptCode>();

                if (arrDeptCode?.Length > 0 && arrDeptName?.Length > 0)
                {
                    for (int i = 0; i < arrDeptCode.Length; i++)
                    {
                        result.DeptCodes.Add(new GetHospitalDetailResultDeptCode
                        {
                            HospKey = req.HospKey,
                            MdCd = arrDeptCode[i],
                            MdNm = arrDeptName[i]
                        });
                    }
                }
            }

            return Result.Success(result);
        }
    }
}
