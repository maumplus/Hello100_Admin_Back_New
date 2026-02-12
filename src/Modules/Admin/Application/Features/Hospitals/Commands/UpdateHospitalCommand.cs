using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Commands
{
    public record UpdateHospitalCommand : IQuery<Result>
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;
        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; init; } = default!;
        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; init; } = default!;
        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; init; } = default!;
        /// <summary>
        /// 대표번호
        /// </summary>
        public string Tel { get; init; } = default!;
        /// <summary>
        /// 홈페이지
        /// </summary>
        public string? Site { get; init; }
        /// <summary>
        /// 위경도 x좌표
        /// </summary>
        public double Lat { get; init; }
        /// <summary>
        /// 위경도 y좌표
        /// </summary>
        public double Lng { get; init; }
        /// <summary>
        /// 차트타입 [E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string? ChartType { get; init; }
        /// <summary>
        /// 테스트병원여부
        /// 0:일반병원
        /// 1:테스트병원
        /// </summary>
        public int IsTest { get; init; }
        /// <summary>
        /// 진료과 코드 목록 (선택한 진료과 코드)
        /// </summary>
        public List<string>? MdCds { get; init; }
    }

    public class UpdateHospitalCommandHandler : IRequestHandler<UpdateHospitalCommand, Result>
    {
        private readonly ILogger<UpdateHospitalCommandHandler> _logger;
        private readonly IHospitalsRepository _hospitalsRepository;
        private readonly ICurrentHospitalProfileProvider _hospitalProfileProvider;
        private readonly IDbSessionRunner _db;

        public UpdateHospitalCommandHandler(
            ILogger<UpdateHospitalCommandHandler> logger,
            IHospitalsRepository hospitalsRepository,
            ICurrentHospitalProfileProvider hospitalProfileProvider,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalsRepository = hospitalsRepository;
            _hospitalProfileProvider = hospitalProfileProvider;
            _db = db;
        }

        public async Task<Result> Handle(UpdateHospitalCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle CreateHospitalCommandHandler");

            var hospInfo = await _hospitalProfileProvider.GetCurrentHospitalProfileByHospKeyAsync(req.HospKey, ct);

            if (hospInfo == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundHospital.ToError());

            var medicalEntity = (req.MdCds ?? Enumerable.Empty<string>())
                               .Select(md => new TbHospitalMedicalInfoEntity { MdCd = md })
                               .ToList();

            var hospitalEntity = req.Adapt<TbHospitalInfoEntity>();

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalsRepository.UpdateHospitalAsync(session, medicalEntity, hospitalEntity, token),
            ct);

            return Result.Success();
        }
    }
}
