using DocumentFormat.OpenXml.Spreadsheet;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Commands;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Commands
{
    public class SetHospNoCommandHandler : IRequestHandler<SetHospNoCommand, Result>
    {
        private readonly ILogger<SetHospNoCommandHandler> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IDbSessionRunner _db;
        private readonly string _paperCheckUrl;

        public SetHospNoCommandHandler(
            ILogger<SetHospNoCommandHandler> logger,
            IAccountRepository accountRepository,
            IDbSessionRunner db,
            IConfiguration config)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _db = db;
            _paperCheckUrl = config["PaperCheckUrl"] ?? string.Empty;
        }

        public async Task<Result> Handle(SetHospNoCommand request, CancellationToken cancellationToken)
        {
            var result = await _db.RunAsync(DataSource.Hello100,
                (dbSession, ct) => _accountRepository.GetEghisHospInfoAsync(dbSession, request.HospNo, ct),
            cancellationToken);

            if (result != null)
            {
                return Result.Success().WithError(AdminErrorCode.ExistsHospMappingInfo.ToError());
            }

            try
            {
                var admin = new TbAdminEntity()
                {
                    Aid = request.Aid,
                    HospNo = request.HospNo
                };

                await _db.RunAsync(DataSource.Hello100,
                    (dbSession, ct) => _accountRepository.UpdateAdminAsync(dbSession, admin, ct),
                cancellationToken);

                var eghisHospInfo = new TbEghisHospInfoEntity()
                {
                    HospKey = request.HospKey,
                    HospNo = request.HospNo,
                    ChartType = request.ChartType
                };

                await _db.RunAsync(DataSource.Hello100,
                    (dbSession, ct) => _accountRepository.UpdateEghisHospInfoAsync(dbSession, eghisHospInfo, ct),
                cancellationToken);

                var eghisHospQrInfo = new TbEghisHospQrInfoEntity()
                {
                    Aid = request.Aid,
                    HospKey = request.HospKey
                };

                await _db.RunAsync(DataSource.Hello100,
                    (dbSession, ct) => _accountRepository.UpdateEghisHospInfoAsync(dbSession, eghisHospQrInfo, ct),
                cancellationToken);

                var eghisRecertDocInfo = new TbEghisRecertDocInfoEntity()
                {
                    HospKey = request.HospKey
                };

                await _db.RunAsync(DataSource.Hello100,
                    (dbSession, ct) => _accountRepository.UpdateEghisHospInfoAsync(dbSession, eghisRecertDocInfo, ct),
                cancellationToken);

                var eghisHospVisitPurposeInfoList = new List<TbEghisHospVisitPurposeInfoEntity>();

                if (request.ChartType == "E")
                {
                    eghisHospVisitPurposeInfoList.Add(
                        new TbEghisHospVisitPurposeInfoEntity()
                        {
                            VpCd = "01",
                            ParentCd = "0",
                            HospKey = request.HospKey,
                            InpuiryUrl = _paperCheckUrl,
                            InpuiryIdx = -1,
                            InpuirySkipYn = "Y",
                            Name = "공단검진",
                            ShowYn = "Y",
                            SortNo = 1,
                            DelYn = "N",
                            Role = 1
                        }
                    );

                    eghisHospVisitPurposeInfoList.Add(
                        new TbEghisHospVisitPurposeInfoEntity()
                        {
                            VpCd = "0101",
                            ParentCd = "0",
                            HospKey = request.HospKey,
                            InpuiryUrl = "",
                            InpuiryIdx = -1,
                            InpuirySkipYn = "Y",
                            Name = "일반검진",
                            ShowYn = "Y",
                            SortNo = 1,
                            DelYn = "N",
                            Role = 1
                        }
                    );

                    eghisHospVisitPurposeInfoList.Add(
                        new TbEghisHospVisitPurposeInfoEntity()
                        {
                            VpCd = "0102",
                            ParentCd = "0",
                            HospKey = request.HospKey,
                            InpuiryUrl = "",
                            InpuiryIdx = -1,
                            InpuirySkipYn = "Y",
                            Name = "암검진",
                            ShowYn = "Y",
                            SortNo = 2,
                            DelYn = "N",
                            Role = 1
                        }
                    );

                    eghisHospVisitPurposeInfoList.Add(
                        new TbEghisHospVisitPurposeInfoEntity()
                        {
                            VpCd = "02",
                            ParentCd = "0",
                            HospKey = request.HospKey,
                            InpuiryUrl = _paperCheckUrl,
                            InpuiryIdx = -1,
                            InpuirySkipYn = "Y",
                            Name = "일반진료",
                            ShowYn = "Y",
                            SortNo = 2,
                            DelYn = "N",
                            Role = 1
                        }
                    );
                }
                else
                {
                    eghisHospVisitPurposeInfoList.Add(
                        new TbEghisHospVisitPurposeInfoEntity()
                        {
                            VpCd = "02",
                            ParentCd = "0",
                            HospKey = request.HospKey,
                            InpuiryUrl = _paperCheckUrl,
                            InpuiryIdx = -1,
                            InpuirySkipYn = "Y",
                            Name = "일반진료",
                            ShowYn = "Y",
                            SortNo = 1,
                            DelYn = "N",
                            Role = 1
                        }
                    );
                }

                await _db.RunAsync(DataSource.Hello100,
                    (dbSession, ct) => _accountRepository.InsertEghisHospVisitPurposeInfoAsync(dbSession, eghisHospVisitPurposeInfoList, ct),
                cancellationToken);

                var eghisHospSettingsInfo = new TbEghisHospSettingsInfoEntity()
                {
                    HospKey = request.HospKey
                };

                await _db.RunAsync(DataSource.Hello100,
                    (dbSession, ct) => _accountRepository.InsertEghisHospSettingsInfoAsync(dbSession, eghisHospSettingsInfo, ct),
                cancellationToken);

                var eghisHospMedicalTimeNewList = new List<TbEghisHospMedicalTimeNewEntity>()
                {
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 1,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    },
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 2,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    },
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 3,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    },
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 4,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    },
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 5,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    },
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 6,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    },
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 7,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    },
                    new TbEghisHospMedicalTimeNewEntity()
                    {
                        HospKey = request.HospKey,
                        HospNo = request.HospNo,
                        WeekNum = 8,
                        StartHour = 9,
                        StartMinute = 0,
                        EndHour = 18,
                        EndMinute = 0,
                        BreakStartHour = 13,
                        BreakStartMinute = 0,
                        BreakEndHour = 14,
                        BreakEndMinute = 0,
                        UseYn = "Y"
                    }
                };

                await _db.RunAsync(DataSource.Hello100,
                   (dbSession, ct) => _accountRepository.InsertTbEghisHospMedicalTimeNewAsync(dbSession, eghisHospMedicalTimeNewList, ct),
                cancellationToken);

                return Result.Success();
            }
            catch
            {
                return Result.Success().WithError(AdminErrorCode.HospitalInfoSaveFaild.ToError());
            }
        }
    }
}
