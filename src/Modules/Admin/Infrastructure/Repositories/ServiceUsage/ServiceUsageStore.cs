using Dapper;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalHistory;
using Microsoft.Extensions.Logging;
using System.Text;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.GetUntactMedicalHistory;
using Mapster;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.ServiceUsage
{
    public class ServiceUsageStore : IServiceUsageStore
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<ServiceUsageStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public ServiceUsageStore(IDbConnectionFactory connection, ILogger<ServiceUsageStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region GENERAL METHOD AREA **************************************
        public async Task<GetUntactMedicalHistoryReadModel?> GetUntactMedicalHistoryAsync(GetUntactMedicalHistoryQuery req, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetUntactMedicalHistoryAsync() Started");

                var fromDate = string.IsNullOrEmpty(req.FromDate) ? null : Convert.ToDateTime(req.FromDate).ToString("yyyyMMdd");
                var toDate = string.IsNullOrEmpty(req.ToDate) ? null : Convert.ToDateTime(req.ToDate).ToString("yyyyMMdd");
                int offset = (req.PageNo - 1) * req.PageSize;

                var parameters = new DynamicParameters();
                parameters.Add("@SearchKeyword", req.SearchKeyword ?? string.Empty, DbType.String);
                parameters.Add("@Limit", req.PageSize, DbType.Int32);
                parameters.Add("@OffSet", offset, DbType.Int32);
                parameters.Add("@FromDt", fromDate, DbType.String);
                parameters.Add("@ToDt", toDate, DbType.String);
                parameters.Add("@HospNo", req.HospNo, DbType.String);
                //parameters.Add("@HospNo", "10350010", DbType.String);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                StringBuilder sbCondi = new StringBuilder();


                if (!string.IsNullOrEmpty(req.SearchKeyword))
                {
                    sbCondi.AppendLine("     and convert(AES_DECRYPT(FROM_BASE64(tm.name) , 'dcc2b29aaa9f271d') USING utf8) like '%" + req.SearchKeyword + "%'                        	");
                }
                if (!string.IsNullOrEmpty(req.FromDate) && !string.IsNullOrEmpty(req.ToDate))
                {
                    //결제일 기준
                    if (req.SearchDateType == "2")
                    {
                        sbCondi.AppendLine("     and from_unixtime(IFNULL(erp.reg_dt,''), '%Y%m%d') between @FromDt AND @ToDt                       	");
                    }
                    else
                    {
                        //진료일 기준
                        sbCondi.AppendLine("     and te.req_date between @FromDt AND @ToDt                       	");
                    }
                }

                sb.AppendLine(" SET @total_count:= (SELECT COUNT(*) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo and te.recept_type='NR' ");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine(" and erp.recept_no > 0 ");
                }
                sb.AppendLine($"{sbCondi.ToString()}    ); ");
                sb.AppendLine(" SET @row_num:=@total_count + 1 - @OffSet ;  ");
                sb.AppendLine("  select         	");
                sb.AppendLine("  @row_num:=@row_num - 1 AS RowNum,	");
                sb.AppendLine("  te.uid as UId,  	");
                sb.AppendLine("  te.mid as MId,  	");
                sb.AppendLine("  (select max(doct_nm) from hello100_api.eghis_doct_info tedi where hosp_no = te.hosp_no and empl_no = te.empl_no) as DoctNm, ");
                sb.AppendLine("      CONCAT(	");
                sb.AppendLine("          SUBSTRING(req_date, 1, 4), '.', 	");
                sb.AppendLine("          SUBSTRING(req_date, 5, 2), '.', 	");
                sb.AppendLine("          SUBSTRING(req_date, 7, 2), ' ',	");
                sb.AppendLine("          SUBSTRING(req_time, 1, 2), ':', 	");
                sb.AppendLine("          SUBSTRING(req_time, 3, 2), ''	");
                sb.AppendLine("      ) AS ReqDate,	");
                sb.AppendLine("  tm.name as Name, 	");
                sb.AppendLine("   te.ptnt_state as PtntState,    	");
                sb.AppendLine("        (select cm_name from hello100.tb_common tc where tc.cls_cd = '26' and tc.cm_cd = te.ptnt_state) as PtntStateNm,	");
                sb.AppendLine("        te.recept_type as ReceptType,  	");
                sb.AppendLine("        tc.cm_name as ReceptTypeNm,	");
                sb.AppendLine("        ifnull(erp.amount,0) as Amount,	");
                sb.AppendLine("        (select cm_name from hello100.tb_common tc where tc.cls_cd = '28' and tc.cm_cd = erp.process_status) as ProcessStatusNm,	");
                sb.AppendLine("        erp.process_status as ProcessStatus,	");
                sb.AppendLine("        (select max(payment_id) from hello100_api.nhn_kcp_payment nkp where nkp.hosp_no=te.hosp_no and nkp.recept_no=te.recept_no) as PaymentId	");
                sb.AppendLine("       from                               	");
                sb.AppendLine("    hello100.tb_eghis_hosp_receipt_info te  	");
                sb.AppendLine("   left outer join hello100_api.eghis_recept_payment erp on te.hosp_no =erp.hosp_no  and te.recept_no =erp.recept_no	");
                sb.AppendLine("   left join hello100.tb_member tm on te.mid = tm.mid  	");
                sb.AppendLine("   left join hello100.tb_common tc on te.recept_type = tc.cm_cd and tc.cls_cd=16   	");
                sb.AppendLine("       where te.hosp_no = @HospNo and te.recept_type='NR'       	");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine($"{sbCondi.ToString()}     ");

                sb.AppendLine("     order by te.req_date desc ,te.serial_no desc                       	");
                sb.AppendLine(" LIMIT @OffSet, @Limit;                      ");
                sb.AppendLine(" SELECT @total_count;                        ");

                //진료예약건수
                sb.AppendLine(" SET @total_rsrv:= (SELECT COUNT(*) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo and te.recept_type='NR'   ");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine(" and  ptnt_state in (1,2,3,4)    ");
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_rsrv;                        ");

                //진료완료건수
                sb.AppendLine(" SET @total_clinic_end:= (SELECT COUNT(*) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo  and te.recept_type='NR' ");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine(" and  ptnt_state >= 9 ");
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_clinic_end;                        ");

                //진료실패
                sb.AppendLine(" SET @total_clinic_fail:= (SELECT COUNT(*) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo and te.recept_type='NR'  ");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine(" and  ptnt_state=7 ");
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_clinic_fail;                        ");

                //진료취소
                sb.AppendLine(" SET @total_clinic_end:= (SELECT COUNT(*) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo and te.recept_type='NR'  ");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine(" and  ptnt_state=8 ");
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_clinic_end;                        ");

                //결제성공금액
                sb.AppendLine(" SET @total_success_amt:= (SELECT IFNULL(SUM(ERP.amount),0) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo  and te.recept_type='NR' and erp.process_status = 9");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_success_amt;                        ");

                //결제진행금액
                sb.AppendLine(" SET @total_progress_amt:= (SELECT IFNULL(SUM(ERP.amount),0) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo  and te.recept_type='NR' and erp.process_status not in (8, 9)");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_progress_amt;                        ");

                //결제실패금액
                sb.AppendLine(" SET @total_fail_amt:= (SELECT IFNULL(SUM(ERP.amount),0) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo  and te.recept_type='NR' and erp.process_status = 8");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_fail_amt;                        ");

                //총결제금액
                sb.AppendLine(" SET @total_sum_amt:= (SELECT IFNULL(SUM(ERP.amount),0) FROM tb_eghis_hosp_receipt_info te left join tb_member tm on te.mid = tm.mid  ");
                sb.AppendLine(" left outer join hello100_api.eghis_recept_payment erp on te.hosp_no = erp.hosp_no  and te.recept_no = erp.recept_no  ");
                sb.AppendLine(" where te.hosp_no = @HospNo  and te.recept_type='NR' ");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine(" );SELECT @total_sum_amt;                        ");
                #endregion

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(sb.ToString(), parameters);

                if (multi != null)
                {
                    var tempResult = new GetUntactMedicalHistoryRow();

                    tempResult.DetailList = (await multi.ReadAsync<GetUntactMedicalHistoryDetailRow>()).ToList();

                    tempResult.TotalCount = await multi.ReadSingleAsync<int>();
                    tempResult.TotalRsrv = await multi.ReadSingleAsync<int>();
                    tempResult.TotalClinicEnd = await multi.ReadSingleAsync<int>();
                    tempResult.TotalClinicFail = await multi.ReadSingleAsync<int>();
                    tempResult.TotalClinicCancel = await multi.ReadSingleAsync<int>();
                    tempResult.TotalSuccessAmt = await multi.ReadSingleAsync<int>();
                    tempResult.TotalProgressAmt = await multi.ReadSingleAsync<int>();
                    tempResult.TotalFailAmt = await multi.ReadSingleAsync<int>();
                    tempResult.TotalSumAmt = await multi.ReadSingleAsync<int>();

                    var result = tempResult.Adapt<GetUntactMedicalHistoryReadModel>();

                    if (result != null && result.DetailList != null && result.DetailList.Count > 0)
                    {
                        var startRowNum = result.TotalCount - (req.PageNo - 1) * req.PageSize;

                        for (int i = 0; i < result.DetailList.Count; i++)
                        {
                            result.DetailList[i].RowNum = startRowNum - i;
                        }
                    }

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetSellerRemitWaitListAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        #endregion
    }
}
