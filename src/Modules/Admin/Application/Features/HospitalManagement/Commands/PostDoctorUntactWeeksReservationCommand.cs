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
    public class PostDoctorUntactWeeksReservationCommand : IRequest<Result>
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public int UntactRsrvIntervalTime { get; set; }
        public int UntactRsrvIntervalCnt { get; set; }
        public string UntactAvaStartTime { get; set; }
        public string UntactAvaEndTime { get; set; }
        public string UntactAvaUseYn { get; set; }
        public List<EghisDoctRsrvDetailInfoEntity> EghisDoctRsrvDetailInfoList { get; set; }
    }

    public class PostDoctorUntactWeeksReservationCommandHandler : IRequestHandler<PostDoctorUntactWeeksReservationCommand, Result>
    {
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly ILogger<PostDoctorUntactWeeksReservationCommandHandler> _logger;
        private readonly IDbSessionRunner _db;

        public PostDoctorUntactWeeksReservationCommandHandler(
        IHospitalManagementRepository hospitalRepository,
        ILogger<PostDoctorUntactWeeksReservationCommandHandler> logger,
        IDbSessionRunner db)
        {
            _hospitalRepository = hospitalRepository;
            _logger = logger;
            _db = db;
        }

        public async Task<Result> Handle(PostDoctorUntactWeeksReservationCommand command, CancellationToken cancellationToken)
        {
            var eghisDoctRsrvInfoEntity = new EghisDoctRsrvInfoEntity()
            {
                HospNo = command.HospNo,
                EmplNo = command.EmplNo,
                ClinicYmd = string.Empty,
                WeekNum = command.WeekNum,
                UntactRsrvIntervalTime = command.UntactRsrvIntervalTime,
                UntactRsrvIntervalCnt = command.UntactRsrvIntervalCnt,
                UntactAvaStartTime = command.UntactAvaStartTime,
                UntactAvaEndTime = command.UntactAvaEndTime,
                UntactAvaUseYn = command.UntactAvaUseYn
            };

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalRepository.RemoveEghisDoctRsrvAsync(session, eghisDoctRsrvInfoEntity, "NR", token);

                var ridx = await _hospitalRepository.InsertEghisDoctUntactRsrvAsync(session, eghisDoctRsrvInfoEntity, token);

                foreach (var eghisDoctRsrvDetailInfoEntity in command.EghisDoctRsrvDetailInfoList)
                {
                    eghisDoctRsrvDetailInfoEntity.Ridx = ridx;
                    eghisDoctRsrvDetailInfoEntity.ReceptType = "NR";
                }

                await _hospitalRepository.InsertEghisDoctRsrvDetailAsync(session, command.EghisDoctRsrvDetailInfoList, token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
