using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNhisHealthScreening
{
    public class UpdateVisitPurposeForNhisHealthScreeningCommandHandler : IRequestHandler<UpdateVisitPurposeForNhisHealthScreeningCommand, Result>
    {
        private readonly ILogger<UpdateVisitPurposeForNhisHealthScreeningCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;
        private readonly IVisitPurposeStore _visitPurposeStore;
        private readonly IDbSessionRunner _db;

        public UpdateVisitPurposeForNhisHealthScreeningCommandHandler(
            ILogger<UpdateVisitPurposeForNhisHealthScreeningCommandHandler> logger,
            IVisitPurposeRepository visitPurposeRepository,
            IVisitPurposeStore visitPurposeStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
            _visitPurposeStore = visitPurposeStore;
            _db = db;
        }

        public async Task<Result> Handle(UpdateVisitPurposeForNhisHealthScreeningCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process UpdateVisitPurposeForNhisHealthScreeningCommandHandler started. HospKey: {HospKey}", req.HospKey);

            var visitPurposes = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _visitPurposeStore.GetVisitPurposeByHospKeyAsync(session, req.HospKey, token),
            ct);

            // 공단검진은 무조건 VpCd "01"
            var purpose = visitPurposes.FirstOrDefault(x => x.VpCd == "01");

            if (purpose != null)
            {
                purpose.ShowYn = req.ShowYn;
                purpose.Role = req.Role;
            }

            #region VALIDATE *************************************
            if (req.ShowYn == "N")
            {
                int purposeViewCount = visitPurposes.Count(x => x.ShowYn == "Y");

                if (purposeViewCount <= 0)
                    return Result.Success().WithError(AdminErrorCode.VisitPurposeExposureRequired.ToError());
            }

            // 병원 자체 모바일접수 체크 항목에 따라, 각 하나씩은 무조건 노출이 필요함.
            var hospRole = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _visitPurposeStore.GetHospRoleByHospKeyAsync(session, req.HospKey, ct),
            ct);

            if (hospRole == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundHospital.ToError());

            var yCodes = visitPurposes.Where(x => x.ShowYn == "Y").Select(x => x.VpCd).ToHashSet();

            bool hasReceiptPurpose = visitPurposes.Where(p => yCodes.Contains(p.VpCd)).Any(p => HasRole(p.Role, 1)); // 1: QR/당일접수
            bool hasReservePurpose = visitPurposes.Where(p => yCodes.Contains(p.VpCd)).Any(p => HasRole(p.Role, 4)); // 4: 예약

            bool receiptRequired = HasRole(hospRole.Role, 1) || HasRole(hospRole.Role, 2); // 1: QR접수 || 2: 당일접수
            bool reserveRequired = HasRole(hospRole.Role, 4); // 4: 진료 예약

            if (receiptRequired && !hasReceiptPurpose)
                return Result.Success().WithError(AdminErrorCode.AtLeastOneQrReceptionVisitPurposeRequired.ToError());

            if (reserveRequired && !hasReservePurpose)
                return Result.Success().WithError(AdminErrorCode.AtLeastOneAppointmentVisitPurposeRequired.ToError());
            #endregion

            await _visitPurposeRepository.UpdateVisitPurposeForNhisHealthScreeningAsync(req.HospKey, req.ShowYn, req.Role, req.DetailShowYn, ct);

            // 현재 운영에서 정상 동작하지 않는 것으로 확인되어 해당 내용 삭제
            // "hello desk update perpose" 라는 PUSH 알림 전송하는 기능
            //if (hospInfo.TabletCnt > 0)
            //    await SendPushDeviceUpdate(userInfo.HospKey, userInfo.HospNo, "purpose");

            _logger.LogInformation("Visit purpose for NHIS health screening updated successfully for HospKey: {HospKey}", req.HospKey);

            return Result.Success();
        }

        private bool HasRole(int role, int flag)
            => (role & flag) != 0;
    }
}
