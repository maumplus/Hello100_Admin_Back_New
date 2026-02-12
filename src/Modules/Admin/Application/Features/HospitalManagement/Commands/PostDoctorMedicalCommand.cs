using DocumentFormat.OpenXml.Office2016.Excel;
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
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public class PostDoctorMedicalCommand : IRequest<Result>
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public List<EghisDoctInfoMdEntity> EghisDoctInfoMdList { get; set; }
    }

    public class PostDoctorMedicalCommandHandler : IRequestHandler<PostDoctorMedicalCommand, Result>
    {
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly ILogger<PostDoctorMedicalCommandHandler> _logger;
        private readonly IDbSessionRunner _db;

        public PostDoctorMedicalCommandHandler(
        IHospitalManagementRepository hospitalRepository,
        ILogger<PostDoctorMedicalCommandHandler> logger,
        IDbSessionRunner db)
        {
            _hospitalRepository = hospitalRepository;
            _logger = logger;
            _db = db;
        }

        public async Task<Result> Handle(PostDoctorMedicalCommand command, CancellationToken cancellationToken)
        {
            var eghisDoctInfoMdEntity = new EghisDoctInfoMdEntity()
            {
                HospNo = command.HospNo,
                EmplNo = command.EmplNo
            };

            foreach (var info in command.EghisDoctInfoMdList)
            {
                info.HospNo = command.HospNo;
                info.HospKey = command.HospKey;
                info.EmplNo = command.EmplNo;
            }

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalRepository.RemoveEghisDoctInfoMdAsync(session, eghisDoctInfoMdEntity, token);

                foreach (var eghisDoctInfoMdInfo in command.EghisDoctInfoMdList)
                {
                    await _hospitalRepository.InsertEghisDoctInfoMdAsync(session, eghisDoctInfoMdInfo, token);
                }
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
