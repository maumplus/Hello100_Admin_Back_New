using DocumentFormat.OpenXml.Spreadsheet;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public class PatchDoctorDaysReservationCommand : IRequest<Result>
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
        public List<EghisDoctRsrvDetailInfoEntity> EghisDoctRsrvDetailInfoList { get; set; }
    }

    public class PatchDoctorDaysReservationCommandHandler : IRequestHandler<PatchDoctorDaysReservationCommand, Result>
    {
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly ILogger<PatchDoctorDaysReservationCommandHandler> _logger;
        private readonly IDbSessionRunner _db;

        public PatchDoctorDaysReservationCommandHandler(
        IHospitalManagementRepository hospitalRepository,
        ILogger<PatchDoctorDaysReservationCommandHandler> logger,
        IDbSessionRunner db)
        {
            _hospitalRepository = hospitalRepository;
            _logger = logger;
            _db = db;
        }

        public async Task<Result> Handle(PatchDoctorDaysReservationCommand command, CancellationToken cancellationToken)
        {
            var eghisDoctRsrvInfoEntity = new EghisDoctRsrvInfoEntity()
            {
                HospNo = command.HospNo,
                EmplNo = command.EmplNo,
                ClinicYmd = command.ClinicYmd,
                WeekNum = 11,
                RsrvIntervalTime = command.RsrvIntervalTime,
                RsrvIntervalCnt = command.RsrvIntervalCnt
            };

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalRepository.RemoveEghisDoctRsrvAsync(session, eghisDoctRsrvInfoEntity, token);

                var ridx = await _hospitalRepository.InsertEghisDoctRsrvAsync(session, eghisDoctRsrvInfoEntity, token);

                foreach (var eghisDoctRsrvDetailInfoEntity in command.EghisDoctRsrvDetailInfoList)
                {
                    eghisDoctRsrvDetailInfoEntity.Ridx = ridx;
                    eghisDoctRsrvDetailInfoEntity.ReceptType = "RS";
                }

                await _hospitalRepository.InsertEghisDoctRsrvDetailAsync(session, command.EghisDoctRsrvDetailInfoList, token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
