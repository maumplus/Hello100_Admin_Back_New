using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
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
    public class PatchDoctorInfo
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public int FrontViewRole { get; set; }
    }

    public class PatchDoctorListCommand : IRequest<Result>
    {
        public string HospNo { get; set; }
        public List<PatchDoctorInfo> DoctorList { get; set; }
    }

    public class PatchDoctorListCommandHandler : IRequestHandler<PatchDoctorListCommand, Result>
    {
        private readonly ILogger<PatchDoctorListCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly ISftpClientService _sftpClientService;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public PatchDoctorListCommandHandler(
            ILogger<PatchDoctorListCommandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            ISftpClientService sftpClientService,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _sftpClientService = sftpClientService;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result> Handle(PatchDoctorListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorListCommand HospNo:{HospNo}", request.HospNo);

            List<EghisDoctInfoEntity> eghisDoctInfoList = new List<EghisDoctInfoEntity>();

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalManagementRepository.UpdateDoctorListAsync(session, eghisDoctInfoList, token),
            cancellationToken);

            return Result.Success();
        }
    }
}
