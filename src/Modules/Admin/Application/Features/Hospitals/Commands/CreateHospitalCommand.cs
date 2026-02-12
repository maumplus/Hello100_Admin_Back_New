using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Commands
{
    public record CreateHospitalCommand : IQuery<Result>
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;
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
        /// 진료과 코드 목록 (선택한 진료과 코드)
        /// </summary>
        public List<string>? MdCds { get; init; }
    }

    public class CreateHospitalCommandHandler : IRequestHandler<CreateHospitalCommand, Result>
    {
        private readonly ILogger<CreateHospitalCommandHandler> _logger;
        private readonly IHospitalsRepository _hospitalsRepository;
        private readonly IDbSessionRunner _db;

        public CreateHospitalCommandHandler(
            ILogger<CreateHospitalCommandHandler> logger, 
            IHospitalsRepository hospitalsRepository, 
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalsRepository = hospitalsRepository;
            _db = db;
        }

        public async Task<Result> Handle(CreateHospitalCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle CreateHospitalCommandHandler");

            var medicalEntity = (req.MdCds ?? Enumerable.Empty<string>())
                               .Select(md => new TbHospitalMedicalInfoEntity { MdCd = md })
                               .ToList();

            var hospitalEntity = req.Adapt<TbHospitalInfoEntity>();

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalsRepository.CreateHospitalAsync(session, req.HospNo, medicalEntity, hospitalEntity, token),
            ct);

            return Result.Success();
        }
    }
}
