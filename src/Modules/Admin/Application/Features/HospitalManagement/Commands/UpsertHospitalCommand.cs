using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    #region DEFINITION COMMAND *******************************
    public record UpsertHospitalCommand : IQuery<Result>
    {
        /// <summary>
        /// 병원명
        /// </summary>
        public string HospNm { get; init; } = default!;
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;
        /// <summary>
        /// 사업자등록번호
        /// </summary>
        public string? BusinessNo { get; init; }
        /// <summary>
        /// 사업자 구분
        /// </summary>
        public string? BusinessLevel { get; init; }
        /// <summary>
        /// 대표번호
        /// </summary>
        public string? Tel { get; init; }
        /// <summary>
        /// 상세정보
        /// </summary>
        public string? Description { get; init; }
        /// <summary>
        /// 대표 진료과
        /// </summary>
        public string? MainMdCd { get; init; }
        /// <summary>
        /// 차트타입 [E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string ChartType { get; init; } = default!;
        /// <summary>
        /// 병원운영시간
        /// </summary>
        public List<MedicalTimeCommandItem>? ClinicTimes { get; init; } = new();
        /// <summary>
        /// 진료시간
        /// </summary>
        public List<MedicalTimeBaseNewCommandItem>? ClinicTimesNew { get; init; } = new();
        /// <summary>
        /// 진료과목
        /// </summary>
        public List<MedicalInfoCommandItem>? DeptCodes { get; init; } = new();
        /// <summary>
        /// 증상/검진 키워드
        /// </summary>
        public List<HashTagInfoCommandItem>? Keywords { get; init; } = new();
        /// <summary>
        /// 기존 이미지정보
        /// </summary>
        public List<ImageInfoCommandItem>? ImgFiles { get; init; } = new();
        /// <summary>
        /// 신규 이미지 목록
        /// </summary>
        public List<FileUploadPayload>? NewImages { get; init; } = new();
    }

    public record UpsertMyHospitalCommand : IQuery<Result>
    {
        public string AId { get; init; } = default!;
        public string HospNo { get; init; } = default!;
        public string HospKey { get; init; } = default!;
        /// <summary>
        /// 사업자등록번호
        /// </summary>
        public string? BusinessNo { get; init; }
        /// <summary>
        /// 사업자구분
        /// </summary>
        public string? BusinessLevel { get; init; }
        /// <summary>
        /// 상세정보
        /// </summary>
        public string? Descrption { get; init; }
        /// <summary>
        /// 대표 진료과
        /// </summary>
        public string? MainMdCd { get; init; }
        /// <summary>
        /// 병원운영시간
        /// </summary>
        public List<MedicalTimeCommandItem>? ClinicTimes { get; init; } = new();
        /// <summary>
        /// 진료시간
        /// </summary>
        public List<MedicalTimeBaseNewCommandItem>? ClinicTimesNew { get; init; } = new();
        /// <summary>
        /// 진료과목
        /// </summary>
        public List<MedicalInfoCommandItem>? DeptCodes { get; init; } = new();
        /// <summary>
        /// 증상/검진 키워드
        /// </summary>
        public List<HashTagInfoCommandItem>? Keywords { get; init; } = new();
        /// <summary>
        /// 기존 이미지정보
        /// </summary>
        public List<ImageInfoCommandItem>? ImgFiles { get; init; } = new();
        /// <summary>
        /// 신규 이미지 목록
        /// </summary>
        public List<FileUploadPayload>? NewImages { get; init; } = new();
    }

    public class MedicalTimeCommandItem
    {
        public int MtId { get; set; }
        public string HospKey { get; set; }
        public string MtNm { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
    }

    public class MedicalTimeBaseNewCommandItem
    {
        public string HospKey { get; set; }
        public string HospNo { get; set; }
        public int WeekNum { get; set; }
        public string WeekNumNm { get; set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }
        public string EndHour { get; set; }
        public string EndMinute { get; set; }
        public string BreakStartHour { get; set; }
        public string BreakStartMinute { get; set; }
        public string BreakEndHour { get; set; }
        public string BreakEndMinute { get; set; }
        public string UseYn { get; set; }
    }

    public class MedicalInfoCommandItem
    {
        public string MdCd { get; set; }
        public string HospKey { get; set; }
        public string MdNm { get; set; }
        public string RegDt { get; set; }
    }

    public class HashTagInfoCommandItem
    {
        public int TagId { get; set; }
        public string Kid { get; set; }
        public string HospKey { get; set; }
        public string TagNm { get; set; }
        public string Keyword { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
        public int MasterSeq { get; set; }
        public int DetailSeq { get; set; }
    }

    public class ImageInfoCommandItem
    {
        public int ImgId { get; set; }
        public string ImgKey { get; set; }
        public string Url { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
    }
    #endregion

    public class UpsertHospitalCommandValidator : AbstractValidator<UpsertHospitalCommand>
    {
        public UpsertHospitalCommandValidator()
        {
            RuleFor(x => x.HospNo).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
            RuleFor(x => x.HospKey).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관 키는 필수입니다.");
            RuleFor(x => x.HospNm).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("병원명은 필수입니다.");
            RuleFor(x => x.ChartType).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("차트정보는 필수입니다.");
        }
    }

    public class UpsertHospitalCommandHandler : IRequestHandler<UpsertHospitalCommand, Result>
    {
        private readonly string _adminImageUrl;
        private readonly ILogger<UpsertHospitalCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly IHospitalInfoProvider _hospitalInfoProvider;
        private readonly ISftpClientService _sftpClientService;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public UpsertHospitalCommandHandler(
            IConfiguration config,
            ILogger<UpsertHospitalCommandHandler> logger,
            IHospitalManagementRepository hospitalRepository,
            IHospitalInfoProvider hospitalInfoProvider,
            ISftpClientService sftpClientService,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _logger = logger;
            _hospitalRepository = hospitalRepository;
            _hospitalInfoProvider = hospitalInfoProvider;
            _sftpClientService = sftpClientService;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result> Handle(UpsertHospitalCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpsertHospitalCommand HospNo:[{HospNo}]", req.HospNo);

            if (req.Keywords is { Count: > 6 })
                return Result.Success().WithError(AdminErrorCode.KeywordSelectionLimitExceeded.ToError());

            var hospInfo = await _hospitalInfoProvider.GetHospitalInfoByHospNoAsync(req.HospNo, ct);

            if (hospInfo == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundCurrentHospital.ToError());

            // Upload Image List
            var customRootDirectory = _cryptoService.EncryptToBase64WithDesEcbPkcs7(req.HospNo);
            var imageUrls = await _sftpClientService.UploadImagesWithPathAsync(req.NewImages, ImageUploadType.HospInfo, customRootDirectory);

            var images = new List<ImageInfoBase>();

            // Add new images
            foreach (var url in imageUrls)
                images.Add(new ImageInfoBase { ImgKey = "", DelYn = "N", Url = url });

            var bizReq = req.Adapt<SaveHospital>();
            bizReq.ClinicTimes = new();
            bizReq.Name = hospInfo.Name;
            bizReq.Images = images;

            var storedImages = bizReq.ImgFiles;

            if (storedImages is { Count: > 0 })
            {
                foreach (var img in storedImages)
                {
                    // 기존 이미지의 경우 admin image url 경로 포함하고 있으니 경로 삭제 필요
                    if (!string.IsNullOrEmpty(img.Url))
                        img.Url = img.Url.Replace(_adminImageUrl, "");

                    img.ImgKey = "";
                    images.Add(img);
                }
            }

            bizReq.ClinicTimesNew?.ForEach(s =>
            {
                s.HospKey = req.HospKey;
                s.HospNo = req.HospNo;
            });

            bizReq.DeptCodes?.ForEach(s =>
            {
                s.HospKey = req.HospKey;
            });

            var clinicTimesNewEntity = bizReq.ClinicTimesNew.Adapt<List<TbEghisHospMedicalTimeNewEntity>>();
            var deptCodesEntity = bizReq.DeptCodes.Adapt<List<TbHospitalMedicalInfoEntity>>();
            var keywordsEntity = bizReq.Keywords.Adapt<List<TbEghisHospKeywordInfoEntity>>();
            var imagesEntity = images.Adapt<List<TbImageInfoEntity>>();

            // Update Database
            await _db.RunInTransactionAsync(DataSource.Hello100, 
                (session, token) => _hospitalRepository.UpsertAdmHospitalAsync(
                    session, req.HospNm, req.HospNo, req.HospKey, req.Tel, req.Description, req.ChartType, req.BusinessNo, req.BusinessLevel, req.MainMdCd,
                     clinicTimesNewEntity, deptCodesEntity, keywordsEntity, imagesEntity, token), 
            ct);

            return Result.Success();
        }

        #region INTERNAL SAVE HOSPITAL MODELS *******************************
        /// <summary>
        /// 해당 클래스 내부에서만 사용하는 내부 클래스
        /// 추후 네이밍 겹칠 경우 해당 클래스명 및 하위 클래스명 수정해도 무방
        /// </summary>
        private sealed class SaveHospital
        {
            public string Name { get; set; }
            public string HospNo { get; set; }
            public string BusinessNo { get; set; }
            public string HospKey { get; set; }
            public string Descrption { get; set; }
            public int Role { get; set; }
            public string ReceptEndTime { get; set; }
            public string Notice { get; set; }
            public List<MedicalTimeBase> ClinicTimes { get; set; } = new();
            public List<MedicalTimeBaseNew> ClinicTimesNew { get; set; }
            public List<MedicalInfoBase> DeptCodes { get; set; }
            public List<HashTagInfoBase> Keywords { get; set; }
            public List<ImageInfoBase> ImgFiles { get; set; }
            public List<ImageInfoBase> Images { get; set; }
            public string JsonData { get; set; }
            public string MainMdCd { get; set; }
            public string BusinessLevel { get; set; }
        }

        private sealed class MedicalTimeBase
        {
            public int MtId { get; set; }
            public string HospKey { get; set; }
            public string MtNm { get; set; }
            public string DelYn { get; set; }
            public string RegDt { get; set; }
        }

        private sealed class MedicalTimeBaseNew
        {
            public string HospKey { get; set; }
            public string HospNo { get; set; }
            public int WeekNum { get; set; }
            public string WeekNumNm { get; set; }
            public string StartHour { get; set; }
            public string StartMinute { get; set; }
            public string EndHour { get; set; }
            public string EndMinute { get; set; }
            public string BreakStartHour { get; set; }
            public string BreakStartMinute { get; set; }
            public string BreakEndHour { get; set; }
            public string BreakEndMinute { get; set; }
            public string UseYn { get; set; }
        }

        private sealed class MedicalInfoBase
        {
            public string MdCd { get; set; }
            public string HospKey { get; set; }
            public string MdNm { get; set; }
            public string RegDt { get; set; }
        }

        private sealed class HashTagInfoBase
        {
            public int TagId { get; set; }
            public string Kid { get; set; }
            public string HospKey { get; set; }
            public string TagNm { get; set; }
            public string Keyword { get; set; }
            public string DelYn { get; set; }
            public string RegDt { get; set; }
            public int MasterSeq { get; set; }
            public int DetailSeq { get; set; }
        }

        private sealed class ImageInfoBase
        {
            public int ImgId { get; set; }
            public string ImgKey { get; set; }
            public string Url { get; set; }
            public string DelYn { get; set; }
            public string RegDt { get; set; }
        }
        #endregion
    }

    public class UpsertMyHospitalCommandHandler : IRequestHandler<UpsertMyHospitalCommand, Result>
    {
        private readonly string _adminImageUrl;
        private readonly ILogger<UpsertMyHospitalCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly IHospitalInfoProvider _hospitalInfoProvider;
        private readonly ISftpClientService _sftpClientService;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public UpsertMyHospitalCommandHandler(
            IConfiguration config,
            ILogger<UpsertMyHospitalCommandHandler> logger,
            IHospitalManagementRepository hospitalRepository,
            IHospitalInfoProvider hospitalInfoProvider,
            ISftpClientService sftpClientService,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _logger = logger;
            _hospitalRepository = hospitalRepository;
            _hospitalInfoProvider = hospitalInfoProvider;
            _sftpClientService = sftpClientService;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result> Handle(UpsertMyHospitalCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpsertMyHospitalCommand HospNo:[{HospNo}]", req.HospNo);

            if (req.Keywords is { Count: > 6 })
                return Result.Success().WithError(AdminErrorCode.KeywordSelectionLimitExceeded.ToError());

            var hospInfo = await _hospitalInfoProvider.GetHospitalInfoByHospNoAsync(req.HospNo, ct);

            if (hospInfo == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundCurrentHospital.ToError());

            // Upload Image List
            var customRootDirectory = _cryptoService.EncryptToBase64WithDesEcbPkcs7(req.HospNo);
            var imageUrls = await _sftpClientService.UploadImagesWithPathAsync(req.NewImages, ImageUploadType.HospInfo, customRootDirectory);

            var images = new List<ImageInfoBase>();

            // Add new images
            foreach (var url in imageUrls)
                images.Add(new ImageInfoBase { ImgKey = "", DelYn = "N", Url = url });

            var bizReq = req.Adapt<SaveHospital>();
            bizReq.ClinicTimes = new();
            bizReq.Name = hospInfo.Name;
            bizReq.Images = images;

            var storedImages = bizReq.ImgFiles;

            if (storedImages is { Count: > 0 })
            {
                foreach (var img in storedImages)
                {
                    // 기존 이미지의 경우 admin image url 경로 포함하고 있으니 경로 삭제 필요
                    if (!string.IsNullOrEmpty(img.Url))
                        img.Url = img.Url.Replace(_adminImageUrl, "");

                    img.ImgKey = "";
                    images.Add(img);
                }
            }

            bizReq.ClinicTimesNew?.ForEach(s =>
            {
                s.HospKey = req.HospKey;
                s.HospNo = req.HospNo;
            });

            bizReq.DeptCodes?.ForEach(s =>
            {
                s.HospKey = req.HospKey;
            });

            var bizReqJson = bizReq.ToJsonForStorage();

            var clinicTimesEntity = bizReq.ClinicTimes.Adapt<List<TbEghisHospMedicalTimeEntity>>();
            var clinicTimesNewEntity = bizReq.ClinicTimesNew.Adapt<List<TbEghisHospMedicalTimeNewEntity>>();
            var deptCodesEntity = bizReq.DeptCodes.Adapt<List<TbHospitalMedicalInfoEntity>>();
            var keywordsEntity = bizReq.Keywords.Adapt<List<TbEghisHospKeywordInfoEntity>>();
            var imagesEntity = images.Adapt<List<TbImageInfoEntity>>();

            // Update Database
            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                var apprId = await _hospitalRepository.UpsertHospitalAsync(session, req.AId, req.HospKey, "HI", bizReqJson, imageUrls.ToList(), token);

                await _hospitalRepository.UpsertAdmMyHospitalAsync
                    (session, req.AId, apprId, req.HospNo, req.HospKey, req.Descrption, req.BusinessNo, req.BusinessLevel, req.MainMdCd,
                    clinicTimesEntity, clinicTimesNewEntity, deptCodesEntity, keywordsEntity, imagesEntity, token);
            }, ct);

            return Result.Success();
        }

        #region INTERNAL SAVE HOSPITAL MODELS *******************************
        /// <summary>
        /// 해당 클래스 내부에서만 사용하는 내부 클래스
        /// 추후 네이밍 겹칠 경우 해당 클래스명 및 하위 클래스명 수정해도 무방
        /// </summary>
        private sealed class SaveHospital
        {
            public string Name { get; set; }
            public string HospNo { get; set; }
            public string BusinessNo { get; set; }
            public string HospKey { get; set; }
            public string Descrption { get; set; }
            public int Role { get; set; }
            public string ReceptEndTime { get; set; }
            public string Notice { get; set; }
            public List<MedicalTimeBase> ClinicTimes { get; set; } = new();
            public List<MedicalTimeBaseNew> ClinicTimesNew { get; set; }
            public List<MedicalInfoBase> DeptCodes { get; set; }
            public List<HashTagInfoBase> Keywords { get; set; }
            public List<ImageInfoBase> ImgFiles { get; set; }
            public List<ImageInfoBase> Images { get; set; }
            public string JsonData { get; set; }
            public string MainMdCd { get; set; }
            public string BusinessLevel { get; set; }
        }

        private sealed class MedicalTimeBase
        {
            public int MtId { get; set; }
            public string HospKey { get; set; }
            public string MtNm { get; set; }
            public string DelYn { get; set; }
            public string RegDt { get; set; }
        }

        private sealed class MedicalTimeBaseNew
        {
            public string HospKey { get; set; }
            public string HospNo { get; set; }
            public int WeekNum { get; set; }
            public string WeekNumNm { get; set; }
            public string StartHour { get; set; }
            public string StartMinute { get; set; }
            public string EndHour { get; set; }
            public string EndMinute { get; set; }
            public string BreakStartHour { get; set; }
            public string BreakStartMinute { get; set; }
            public string BreakEndHour { get; set; }
            public string BreakEndMinute { get; set; }
            public string UseYn { get; set; }
        }

        private sealed class MedicalInfoBase
        {
            public string MdCd { get; set; }
            public string HospKey { get; set; }
            public string MdNm { get; set; }
            public string RegDt { get; set; }
        }

        private sealed class HashTagInfoBase
        {
            public int TagId { get; set; }
            public string Kid { get; set; }
            public string HospKey { get; set; }
            public string TagNm { get; set; }
            public string Keyword { get; set; }
            public string DelYn { get; set; }
            public string RegDt { get; set; }
            public int MasterSeq { get; set; }
            public int DetailSeq { get; set; }
        }

        private sealed class ImageInfoBase
        {
            public int ImgId { get; set; }
            public string ImgKey { get; set; }
            public string Url { get; set; }
            public string DelYn { get; set; }
            public string RegDt { get; set; }
        }
        #endregion
    }
}
