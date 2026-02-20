using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public class PostDoctorWeeksReservationCommand : IRequest<Result>
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
        public List<EghisDoctRsrvDetailInfoEntity> EghisDoctRsrvDetailInfoList { get; set; }
    }

    public class PostDoctorWeeksReservationCommandHandler : IRequestHandler<PostDoctorWeeksReservationCommand, Result>
    {
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly ILogger<PostDoctorWeeksReservationCommandHandler> _logger;
        private readonly IDbSessionRunner _db;

        public PostDoctorWeeksReservationCommandHandler(
        IHospitalManagementRepository hospitalRepository,
        ILogger<PostDoctorWeeksReservationCommandHandler> logger,
        IDbSessionRunner db)
        {
            _hospitalRepository = hospitalRepository;
            _logger = logger;
            _db = db;
        }

        public async Task<Result> Handle(PostDoctorWeeksReservationCommand command, CancellationToken cancellationToken)
        {
            var eghisDoctRsrvInfoEntity = new EghisDoctRsrvInfoEntity()
            {
                HospNo = command.HospNo,
                EmplNo = command.EmplNo,
                ClinicYmd = string.Empty,
                WeekNum = command.WeekNum,
                RsrvIntervalTime = command.RsrvIntervalTime,
                RsrvIntervalCnt = command.RsrvIntervalCnt
            };

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalRepository.RemoveEghisDoctRsrvAsync(session, eghisDoctRsrvInfoEntity, "RS", token);

                var ridx = await _hospitalRepository.InsertEghisDoctRsrvAsync(session, eghisDoctRsrvInfoEntity, token);

                await _hospitalRepository.InsertEghisDoctRsrvDetailAsync(session, command.EghisDoctRsrvDetailInfoList, ridx, "RS", token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
