using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PatchDoctorCammand : IQuery<Result>
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public string DoctNm { get; set; }
        public string ViewMinCntYn { get; set; }
        public string ViewMinCnt { get; set; }
        public string ViewMinTimeYn { get; set; }
        public string ViewMinTime { get; set; }
        public FileUploadPayload? Image { get; set; }
    }

    public class PatchDoctorCammandHandler : IRequestHandler<PatchDoctorCammand, Result>
    {
        private readonly ILogger<PatchDoctorCammandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IHospitalManagementStore _hospitalManagementStore;
        private readonly ISftpClientService _sftpClientService;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public PatchDoctorCammandHandler(
            ILogger<PatchDoctorCammandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IHospitalManagementStore hospitalManagementStore,
            ISftpClientService sftpClientService,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _hospitalManagementStore = hospitalManagementStore;
            _sftpClientService = sftpClientService;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result> Handle(PatchDoctorCammand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorCammand HospNo:{HospNo}", request.HospNo);

            string imagePath = string.Empty;

            if (request.Image != null)
            {
                imagePath = await _sftpClientService.UploadImageWithPathAsync(request.Image, ImageUploadType.UNTACT, _cryptoService.EncryptToBase64WithDesEcbPkcs7(request.HospNo), cancellationToken);
            }

            var doctorList = await _hospitalManagementStore.GetDoctorList(request.HospNo, request.EmplNo, cancellationToken);

            if (doctorList.Count == 0)
            {
                return Result.Success().WithError(AdminErrorCode.NotFoundDoctorInfo.ToError());
            }

            int viewRole = 0;

            if (request.ViewMinCntYn == "Y")
            {
                viewRole = viewRole | 1;
            }

            if (request.ViewMinTimeYn == "Y")
            {
                viewRole = viewRole | 2;
            }

            var eghisDoctInfoEntity = new EghisDoctInfoEntity
            {
                HospNo = request.HospNo,
                HospKey = request.HospKey,
                EmplNo = request.EmplNo,
                DoctNm = request.DoctNm,
                ViewRole = viewRole,
                ViewMinCnt = request.ViewMinCnt,
                ViewMinTime = request.ViewMinTime
            };

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalManagementRepository.UpdateDoctorInfoAsync(session, eghisDoctInfoEntity, token);

                if (request.Image != null && !string.IsNullOrEmpty(imagePath))
                {
                    var tbFileInfoEntity = new TbFileInfoEntity
                    {
                        ClsCd = "24",
                        CmCd = "U04",
                        FilePath = imagePath,
                        FileSize = request.Image.Length,
                        OriginFileName = request.Image.FileName,
                        DelYn = "N"
                    };

                    var tbEghisDoctInfoFileEntity = new TbEghisDoctInfoFileEntity
                    {
                        HospNo = request.HospNo,
                        HospKey = request.HospKey,
                        EmplNo = request.EmplNo,
                        TbFileInfoEntity = tbFileInfoEntity
                    };

                    await _hospitalManagementRepository.UpdateDoctorInfoFileAsync(session, tbEghisDoctInfoFileEntity, token);
                }
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
