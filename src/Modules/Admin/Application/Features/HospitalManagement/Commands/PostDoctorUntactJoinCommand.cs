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
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PostDoctorUntactJoinCommand : IRequest<Result>
    {
        public string HospNo { get; init; } = default!;
        public string EmplNo { get; init; } = default!;
        public string DoctNo { get; init; } = default!;
        public string DoctNoType { get; init; } = default!;
        public string DoctNm { get; init; } = default!;
        public string DoctBirthday { get; init; } = default!;
        public string DoctTel { get; init; } = default!;
        public string DoctIntro { get; init; } = default!;
        public List<string> DoctHistoryList { get; init; } = default!;
        public string? ClinicTime { get; init; }
        public string? ClinicGuide { get; init; }
        public List<FileUploadPayload>? Images { get; init; }
    }

    public class PostDoctorUntactJoinHandler : IRequestHandler<PostDoctorUntactJoinCommand, Result>
    {
        private readonly ILogger<PostDoctorUntactJoinHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IHospitalManagementStore _hospitalManagementStore;
        private readonly ISftpClientService _sftpClientService;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public PostDoctorUntactJoinHandler(
            ILogger<PostDoctorUntactJoinHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IHospitalManagementStore hospitalManagementStore,
            ISftpClientService sftpClientService,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementStore = hospitalManagementStore;
            _hospitalManagementRepository = hospitalManagementRepository;
            _sftpClientService = sftpClientService;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result> Handle(PostDoctorUntactJoinCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PostDoctorUntactJoinCommand HospNo:{HospNo}", request.HospNo);

            var hospInfo = await _hospitalManagementStore.GetHospitalAsync(request.HospNo, cancellationToken);

            if (hospInfo == null)
            {
                return Result.Success().WithError(AdminErrorCode.NotFoundCurrentHospital.ToError());
            }

            var doctHistoryJson = string.Empty;

            foreach (var doctHistory in request.DoctHistoryList)
            {
                doctHistoryJson += string.IsNullOrEmpty(doctHistoryJson) ? "" : ", ";
                doctHistoryJson += $"{{ \"history\": \"{doctHistory}\" }}";
            }

            List<TbFileInfoEntity> fileInfoList = new List<TbFileInfoEntity>();

            if (request.Images != null)
            {
                for (int i = 0; i < request.Images.Count; i++)
                {
                    if (request.Images[i] != null)
                    {
                        var imagePath = await _sftpClientService.UploadImageWithPathAsync(request.Images[i], ImageUploadType.UNTACT, _cryptoService.EncryptToBase64WithDesEcbPkcs7(request.HospNo), cancellationToken);

                        var fileInfoEntity = new TbFileInfoEntity
                        {
                            ClsCd = "24",
                            CmCd = $"U0{i+1}",
                            FilePath = imagePath,
                            FileSize = request.Images[i].Length,
                            OriginFileName = request.Images[i].FileName,
                            DelYn = "N"
                        };

                        fileInfoList.Add(fileInfoEntity);
                    }
                }
            }

            var eghisDoctUntactJoinEntity = new TbEghisDoctUntactJoinEntity
            {
                HospNo = request.HospNo,
                HospKey = hospInfo.HospKey,
                HospNm = hospInfo.Name,
                EmplNo = request.EmplNo,
                HospTel = hospInfo.Tel,
                PostCd = hospInfo.PostCd,
                DoctNo = request.DoctNo,
                DoctNoType = request.DoctNoType,
                DoctLicenseFileInfo = fileInfoList.Count > 0 ? fileInfoList[0] : null,
                DoctNm = request.DoctNm,
                DoctBirthday = request.DoctBirthday,
                DoctTel = request.DoctTel,
                DoctIntro = request.DoctIntro,
                DoctFileSeqInfo = fileInfoList.Count > 1 ? fileInfoList[1] : null,
                DoctHistory = $"[{doctHistoryJson}]",
                ClinicTime = request.ClinicTime,
                ClinicGuide = request.ClinicGuide,
                AccountInfoFileInfo = fileInfoList.Count > 2 ? fileInfoList[2] : null,
                BusinessFileInfo = fileInfoList.Count > 3 ? fileInfoList[3] : null,
                JoinState = "01"
            };

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalManagementRepository.InsertEghisDoctUntactJoinAsync(session, eghisDoctUntactJoinEntity, token),
            cancellationToken);

            return Result.Success();
        }
    }
}
