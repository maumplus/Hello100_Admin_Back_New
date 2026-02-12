using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PatchDoctorUntactCammand : IQuery<Result>
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public string DoctIntro { get; set; }
        public string ClinicGuide { get; set; }
        public List<string> DoctHistoryList { get; set; }
    }

    public class PatchDoctorUntactCammandHandler : IRequestHandler<PatchDoctorUntactCammand, Result>
    {
        private readonly ILogger<PatchDoctorUntactCammandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IDbSessionRunner _db;

        public PatchDoctorUntactCammandHandler(
            ILogger<PatchDoctorUntactCammandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(PatchDoctorUntactCammand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorUntactCammand HospNo:{HospNo}", request.HospNo);

            var doctHistoryJson = string.Empty;

            foreach (var doctHistory in request.DoctHistoryList)
            {
                doctHistoryJson += string.IsNullOrEmpty(doctHistoryJson) ? "" : ", ";
                doctHistoryJson += $"{{ \"history\": \"{doctHistory}\" }}";
            }

            var eghisDoctUntanct = new TbEghisDoctUntanctEntity
            {
                HospNo = request.HospNo,
                DoctIntro = request.DoctIntro,
                ClinicGuide = request.ClinicGuide,
                DoctHistory = $"[{doctHistoryJson}]"
            };

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalManagementRepository.UpdateDoctorUntanctAsync(session, eghisDoctUntanct, token);
            }
            , cancellationToken);

            return Result.Success();
        }
    }
}
