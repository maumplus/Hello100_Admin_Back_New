using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNonNhisHealthScreening
{
    public class UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler : IRequestHandler<UpdateVisitPurposeForNonNhisHealthScreeningCommand, Result>
    {
        private readonly ILogger<UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;
        private readonly IVisitPurposeStore _visitPurposeStore;
        private readonly IDbSessionRunner _db;

        public UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler(
            ILogger<UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler> logger,
            IVisitPurposeRepository visitPurposeRepository,
            IVisitPurposeStore visitPurposeStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
            _visitPurposeStore = visitPurposeStore;
            _db = db;
        }

        public async Task<Result> Handle(UpdateVisitPurposeForNonNhisHealthScreeningCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process UpdateVisitPurposeForNonNhisHealthScreeningCommandHandler started.");

            if (req.ShowYn == "N")
            {
                var visitPurpose = await _visitPurposeStore.GetVisitPurposesAsync(req.HospKey, ct);

                int purposeViewCount = visitPurpose.DetailList.Count(x => x.ShowYn == "Y");

                if (purposeViewCount == 1)
                    return Result.Success().WithError(AdminErrorCode.VisitPurposeExposureRequired.ToError());
            }

            // 병원 자체 모바일접수 체크 항목에 따라, 각 하나씩은 무조건 노출이 필요함.
            var hospRole = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _visitPurposeStore.GetHospRoleByHospKeyAsync(session, req.HospKey, ct),
            ct);

            if (hospRole == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundHospital.ToError());

            // 해당 병원 내원목적 리스트의 Role과 병원 자체의 Role을 비교해서, 하나의 Row라도 일치하는지 체크 필요. (하나도 일치하지 않으면, 앱에서 노출이 안되는 상황이 발생할 수 있기 때문)
            var visitPurposes = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _visitPurposeStore.GetVisitPurposeByVpCdAsync(session, req.HospKey, ct),
            ct);

            var purpose = visitPurposes.FirstOrDefault(x => x.VpCd == req.VpCd);

            if (purpose != null)
            {
                purpose.ShowYn = req.ShowYn;
                purpose.Role = req.Role;
            }

            var yCodes = visitPurposes.Where(x => x.ShowYn == "Y").Select(x => x.VpCd).ToHashSet();

            bool hasReceiptPurpose = visitPurposes.Where(p => yCodes.Contains(p.VpCd)).Any(p => HasRole(p.Role, 1)); // 1: QR/당일접수
            bool hasReservePurpose = visitPurposes.Where(p => yCodes.Contains(p.VpCd)).Any(p => HasRole(p.Role, 4)); // 4: 예약

            bool receiptRequired = HasRole(hospRole.Role, 1) || HasRole(hospRole.Role, 2); // 1: QR접수 || 2: 당일접수
            bool reserveRequired = HasRole(hospRole.Role, 4); // 4: 진료 예약

            if (receiptRequired && !hasReceiptPurpose)
                return Result.Success().WithError(AdminErrorCode.AtLeastOneQrReceptionVisitPurposeRequired.ToError());

            if (reserveRequired && !hasReservePurpose)
                return Result.Success().WithError(AdminErrorCode.AtLeastOneAppointmentVisitPurposeRequired.ToError());


            await _db.RunInTransactionAsync(DataSource.Hello100,
                (session, token) => _visitPurposeRepository.UpdateVisitPurposeForNonNhisHealthScreeningAsync(
                    session, req.VpCd, req.HospKey, req.InpuiryIdx, req.Name, req.ShowYn, req.DetailYn, req.Role, req.Details, token),
            ct);


            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            _logger.LogInformation("Visit purpose for non-NHIS health screening updated successfully.");
            return Result.Success();
        }

        private bool HasRole(int role, int flag)
            => (role & flag) != 0;
    }

    public class UpdateMyVisitPurposeForNonNhisHealthScreeningCommandHandler : IRequestHandler<UpdateMyVisitPurposeForNonNhisHealthScreeningCommand, Result>
    {
        private readonly ILogger<UpdateMyVisitPurposeForNonNhisHealthScreeningCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;
        private readonly IVisitPurposeStore _visitPurposeStore;
        private readonly IDbSessionRunner _db;
        private readonly ICryptoService _cryptoService;

        public UpdateMyVisitPurposeForNonNhisHealthScreeningCommandHandler(
            ILogger<UpdateMyVisitPurposeForNonNhisHealthScreeningCommandHandler> logger,
            IVisitPurposeRepository visitPurposeRepository,
            IVisitPurposeStore visitPurposeStore,
            IDbSessionRunner db,
            ICryptoService cryptoService)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
            _visitPurposeStore = visitPurposeStore;
            _db = db;
            _cryptoService = cryptoService;
        }

        public async Task<Result> Handle(UpdateMyVisitPurposeForNonNhisHealthScreeningCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process UpdateMyVisitPurposeForNonNhisHealthScreeningCommandHandler started.");

            if (req.ShowYn == "N")
            {
                var visitPurpose = await _visitPurposeStore.GetVisitPurposesAsync(req.HospKey, ct);

                int purposeViewCount = visitPurpose.DetailList.Count(x => x.ShowYn == "Y");

                if (purposeViewCount == 1)
                    return Result.Success().WithError(AdminErrorCode.VisitPurposeExposureRequired.ToError());
            }

            // 병원 자체 모바일접수 체크 항목에 따라, 각 하나씩은 무조건 노출이 필요함.
            var hospRole = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _visitPurposeStore.GetHospRoleByHospKeyAsync(session, req.HospKey, ct),
            ct);

            if (hospRole == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundHospital.ToError());

            // 해당 병원 내원목적 리스트의 Role과 병원 자체의 Role을 비교해서, 하나의 Row라도 일치하는지 체크 필요. (하나도 일치하지 않으면, 앱에서 노출이 안되는 상황이 발생할 수 있기 때문)
            var visitPurposes = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _visitPurposeStore.GetVisitPurposeByVpCdAsync(session, req.HospKey, ct),
            ct);

            var purpose = visitPurposes.FirstOrDefault(x => x.VpCd == req.VpCd);

            if (purpose != null)
            {
                purpose.ShowYn = req.ShowYn;
                purpose.Role = req.Role;
            }

            var yCodes = visitPurposes.Where(x => x.ShowYn == "Y").Select(x => x.VpCd).ToHashSet();

            bool hasReceiptPurpose = visitPurposes.Where(p => yCodes.Contains(p.VpCd)).Any(p => HasRole(p.Role, 1)); // 1: QR/당일접수
            bool hasReservePurpose = visitPurposes.Where(p => yCodes.Contains(p.VpCd)).Any(p => HasRole(p.Role, 4)); // 4: 예약

            bool receiptRequired = HasRole(hospRole.Role, 1) || HasRole(hospRole.Role, 2); // 1: QR접수 || 2: 당일접수
            bool reserveRequired = HasRole(hospRole.Role, 4); // 4: 진료 예약

            if (receiptRequired && !hasReceiptPurpose)
                return Result.Success().WithError(AdminErrorCode.AtLeastOneQrReceptionVisitPurposeRequired.ToError());

            if (reserveRequired && !hasReservePurpose)
                return Result.Success().WithError(AdminErrorCode.AtLeastOneAppointmentVisitPurposeRequired.ToError());

            var data = new UpdateVisitPurposeForNonNhisHealthScreeningParams();

            data.Purpose = req.Adapt<UpdateVisitPurposeBizParams>();
            data.Purpose.TranId = _cryptoService.EncryptToBase64WithDesEcbPkcs7(req.VpCd);

            if (req.DetailYn == "Y" && req.Details != null)
            {
                req.Details.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        data.Details.Add(new UpdateVisitPurposeBizDetailsParams
                        {
                            Name = x
                        });
                    }
                });
            }

            await _db.RunInTransactionAsync(DataSource.Hello100, 
                async (session, token) =>
                {
                    // 승인요청정보 테이블에 저장 [ApprovalType HR: 병원 내원정보]
                    var createdApprId = await _visitPurposeRepository.CreateVisitPurposeApprovalAsync(session, req.HospKey, "HR", data.ToJsonForStorage(), req.AId, token);

                    // 이지스병원내원목적정보 테이블에 저장
                    await _visitPurposeRepository.UpdateMyVisitPurposeForNonNhisHealthScreeningAsync(session, req, createdApprId, token);
            
                }, ct);


            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            _logger.LogInformation("Visit purpose for non-NHIS health screening updated successfully.");
            return Result.Success();
        }

        private bool HasRole(int role, int flag)
            => (role & flag) != 0;
    }
}
