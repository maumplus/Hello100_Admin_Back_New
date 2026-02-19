using Dapper;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchUntactMedicalHistories;
using Microsoft.Extensions.Logging;
using System.Text;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchUntactMedicalHistories;
using Mapster;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.GetUntactMedicalPaymentDetail;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportUntactMedicalHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportUntactMedicalHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Constants;

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

        #region ISERVICEUSAGESTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<SearchUntactMedicalHistoriesReadModel> SearchUntactMedicalHistoriesAsync(SearchUntactMedicalHistoriesQuery req, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("SearchUntactMedicalHistoriesAsync() Started");

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

                var result = new SearchUntactMedicalHistoriesReadModel();

                var queryList = (await multi.ReadAsync<SearchUntactMedicalHistoriesRow>()).ToList();

                result.TotalCount = await multi.ReadSingleAsync<int>();
                result.TotalRsrv = await multi.ReadSingleAsync<int>();
                result.TotalClinicEnd = await multi.ReadSingleAsync<int>();
                result.TotalClinicFail = await multi.ReadSingleAsync<int>();
                result.TotalClinicCancel = await multi.ReadSingleAsync<int>();
                result.TotalSuccessAmt = await multi.ReadSingleAsync<int>();
                result.TotalProgressAmt = await multi.ReadSingleAsync<int>();
                result.TotalFailAmt = await multi.ReadSingleAsync<int>();
                result.TotalSumAmt = await multi.ReadSingleAsync<int>();

                result.DetailList = queryList.Adapt<List<SearchUntactMedicalHistoryItemReadModel>>();

                if (result.DetailList != null && result.DetailList.Count > 0)
                {
                    var startRowNum = result.TotalCount - (req.PageNo - 1) * req.PageSize;

                    for (int i = 0; i < result.DetailList.Count; i++)
                    {
                        result.DetailList[i].RowNum = startRowNum - i;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error SearchUntactMedicalHistoriesAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<GetUntactMedicalPaymentDetailReadModel?> GetUntactMedicalPaymentDetailAsync(string paymentId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetUntactMedicalPaymentDetailAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("@PaymentId", paymentId, DbType.String);

                var query = @"
                    SELECT nkp.payment_id                                     AS PaymentId,
                           nkp.hosp_no                                        AS HospNo,
                           nkp.recept_no                                      AS ReceptNo,
                           nkp.ordr_idxx                                      AS OrderIdxx,
                           nkp.process_status                                 AS ProcessStatus,
                           ( SELECT cm_name 
                               FROM hello100.tb_common tc 
                              WHERE tc.cls_cd = '28' 
                                AND tc.cm_cd = nkp.process_status )           AS ProcessStatusNm,
                           Failed_msg                                         AS FailedMsg,
                           res_cd                                             AS ResCd,
                           res_msg                                            AS ResMsg,
                           tnkbk.card_cd                                      AS CardCd,
                           tnkbk.card_name                                    AS CardName,
                           tnkbk.card_mask_no                                 AS CardNo,
                           DATE_FORMAT(FROM_UNIXTIME(tnkbk.reg_dt), '%Y%m%d') AS AppTime,
                           nkp.acqu_name                                      AS AcQuname
                      FROM hello100_api.nhn_kcp_payment nkp 
                      LEFT JOIN hello100.tb_nhn_kcp_batch_key tnkbk 
                             ON nkp.batch_key = tnkbk.batch_key
                     WHERE nkp.payment_id = @PaymentId
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<GetUntactMedicalPaymentDetailRow>(query, parameters);

                var config = new TypeAdapterConfig();
                config.NewConfig<GetUntactMedicalPaymentDetailRow, GetUntactMedicalPaymentDetailReadModel>()
                      .Map(d => d.PaymentId, s => s.PaymentId.ToString())
                      .Map(d => d.ProcessStatus, s => s.ProcessStatus.ToString());

                var result = queryResult?.Adapt<GetUntactMedicalPaymentDetailReadModel>(config);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetUntactMedicalPaymentDetailAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<List<GetUntactMedicalHistoryForExportReadModel>> GetUntactMedicalHistoryForExportAsync(ExportUntactMedicalHistoriesExcelQuery req, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetUntactMedicalHistoryForExportAsync() Started");

                var fromDt = string.IsNullOrEmpty(req.FromDate) ? null : Convert.ToDateTime(req.FromDate).ToString("yyyyMMdd");
                var toDt = string.IsNullOrEmpty(req.ToDate) ? null : Convert.ToDateTime(req.ToDate).ToString("yyyyMMdd");

                var parameters = new DynamicParameters();
                parameters.Add("@SearchKeyword", req.SearchKeyword, DbType.String);
                parameters.Add("@FromDt", fromDt, DbType.String);
                parameters.Add("@ToDt", toDt, DbType.String);
                parameters.Add("@HospNo", req.HospNo, DbType.String);

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

                sb.AppendLine("  select         	");
                sb.AppendLine("  CONVERT(AES_DECRYPT(FROM_BASE64(tm.name) , 'dcc2b29aaa9f271d') USING utf8) AS Name, 	");
                sb.AppendLine("  CONCAT(SUBSTRING(te.req_date  , 1, 4), '.',SUBSTRING(te.req_date  , 5, 2), '.',SUBSTRING(te.req_date  , 7, 2)) AS ReqDate,  	");
                sb.AppendLine("        tc.cm_name as ReceiptType,	");
                sb.AppendLine("  (select max(doct_nm) from hello100_api.eghis_doct_info tedi where hosp_no = te.hosp_no and empl_no = te.empl_no) as DoctNm, ");
                sb.AppendLine("        (select cm_name from hello100.tb_common tc where tc.cls_cd = '28' and tc.cm_cd = erp.process_status) as ProcessStatus,	");
                sb.AppendLine("        ifnull(erp.amount,0) as Amount,	");
                sb.AppendLine("        (select cm_name from hello100.tb_common tc where tc.cls_cd = '26' and tc.cm_cd = te.ptnt_state) as PtntState	");
                sb.AppendLine("       from                               	");
                sb.AppendLine("    hello100.tb_eghis_hosp_receipt_info te  	");
                sb.AppendLine("   left outer join hello100_api.eghis_recept_payment erp on te.hosp_no =erp.hosp_no  and te.recept_no =erp.recept_no	");
                sb.AppendLine("   left join hello100.tb_member tm on te.mid = tm.mid  	");
                sb.AppendLine("   left join hello100.tb_common tc on te.recept_type = tc.cm_cd and tc.cls_cd=16   	");
                sb.AppendLine("       where te.hosp_no = @HospNo  and te.recept_type='NR'     	");
                if (req.SearchDateType == "2")
                {
                    sb.AppendLine("   and erp.recept_no > 0       ");
                }
                sb.AppendLine($"{sbCondi.ToString()}     ");
                sb.AppendLine("     order by te.req_date desc                          	");
                #endregion

                using var connection = _connection.CreateConnection();
                var queryResult = (await connection.QueryAsync<GetUntactMedicalHistoryForExportRow>(sb.ToString(), parameters)).ToList();

                var result = queryResult.Adapt<List<GetUntactMedicalHistoryForExportReadModel>>();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetUntactMedicalHistoryForExportAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        // GetExaminationResultAlimtalkHistoryForExportAsync() 과 쿼리는 동일 Page Offset만 다른데. 추후 합치는 방안 고려 usePagenaion true/false 추가?
        // Key값 하드코딩된 부분도 추후 개선 필요
        public async Task<SearchExaminationResultAlimtalkHistoriesReadModel> SearchExaminationResultAlimtalkHistoriesAsync(SearchExaminationResultAlimtalkHistoriesQuery req, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("SearchExaminationResultAlimtalkHistoriesAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("@SearchKeyword", req.SearchKeyword ?? string.Empty, DbType.String);
                parameters.Add("@Limit", req.PageSize, DbType.Int32);
                parameters.Add("@OffSet", (req.PageNo - 1) * req.PageSize, DbType.Int32);
                parameters.Add("@FromDt", Convert.ToDateTime(req.FromDate).ToString("yyyyMMdd"), DbType.String);
                parameters.Add("@ToDt", Convert.ToDateTime(req.ToDate).ToString("yyyyMMdd"), DbType.String);
                parameters.Add("@NameKey", "dcc2b29aaa9f271d", DbType.String); // AES256Key_Email_Name
                parameters.Add("@DecKey", "08a0d3a6ec32e85e", DbType.String); // AES256Key
                parameters.Add("@HospNo", req.HospNo, DbType.String);

                string dateColumn = "erern.regDate";

                if (req.SearchDateType == 1) dateColumn = "erern.regDate";
                else if (req.SearchDateType == 2) dateColumn = @"erern.`date`";

                var conditions = new List<List<string>>()
                {
                    new List<string>()
                    {
                        "erern.hospNo = @HospNo",
                        $"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt"
                    },
                    new List<string>()
                    {
                        "erern.hospNo = @HospNo",
                        $"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt"
                    }
                };

                // 검색어
                if (string.IsNullOrWhiteSpace(req.SearchKeyword) == false)
                {
                    conditions[0].Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(tm.name), @NameKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                    conditions[1].Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(erern.ptntNm), @DecKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("SET block_encryption_mode = 'aes-128-cbc';");

                sb.AppendLine($"SELECT ROW_NUMBER() OVER(ORDER BY a.`Date` DESC, a.ReceptNo DESC) AS RowNum, a.*");
                sb.AppendLine($"  FROM (");
                sb.AppendLine(@"         SELECT CONVERT(AES_DECRYPT(FROM_BASE64(tm.name), 'dcc2b29aaa9f271d', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntName");
                sb.AppendLine(@"              , CONVERT(AES_DECRYPT(FROM_BASE64(tm.sex), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntSex");
                sb.AppendLine($"              , erern.`date` AS `Date`");
                sb.AppendLine($"              , erern.receptNo AS ReceptNo");
                sb.AppendLine($"              , DATE_FORMAT(STR_TO_DATE(erern.`date`, '%Y%m%d'), '%Y.%m.%d') AS ReqDate");
                sb.AppendLine($"              , DATE_FORMAT(erern.regDate, '%Y.%m.%d %T') AS SendDate");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '미발송' WHEN erern.pushYn = 'Y' THEN '-' END AS SendStatus");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '카카오톡' WHEN erern.pushYn = 'Y' THEN 'App Push' end AS SendType");
                sb.AppendLine($"              , CASE");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.ptntNm IS NULL OR erern.ptntNm = '' OR erern.ptntNm = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.phone IS NULL OR erern.phone = '' OR erern.phone = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.examPushSet IS NULL OR erern.examPushSet NOT IN (1, 2, 4)) THEN '검사결과 알림 서비스 사용 안함 상태입니다. 병원정보관리>헬로100설정에서 설정할 수 있습니다.'");
                sb.AppendLine($"                END AS Message");
                sb.AppendLine($"              , HEX(erern.notificationId) as NotificationId");
                sb.AppendLine($"           FROM hello100_api.eghis_recept_examination_result_notification erern");
                sb.AppendLine($"           LEFT JOIN hello100_api.eghis_recept_examination_result erer ON erer.hospNo = erern.hospNo AND erer.receptNo = erern.receptNo AND erer.date = erern.date");
                sb.AppendLine($"          INNER JOIN tb_eghis_hosp_receipt_info teri ON teri.hosp_no = erern.hospNo AND teri.recept_no = erern.receptNo");
                sb.AppendLine($"          INNER JOIN tb_member tm on tm.uid = teri.uid AND tm.mid = teri.mid");
                sb.AppendLine($"          WHERE {(conditions[0].Any() ? $"{string.Join($"{Environment.NewLine}            AND ", conditions[0])}" : string.Empty)}");
                sb.AppendLine($"         UNION ALL");
                sb.AppendLine(@"         SELECT CONVERT(AES_DECRYPT(FROM_BASE64(erern.ptntNm), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntName");
                sb.AppendLine($"              , '-' AS PtntSex");
                sb.AppendLine($"              , erern.`date` AS `Date`");
                sb.AppendLine($"              , erern.receptNo AS ReceptNo");
                sb.AppendLine($"              , DATE_FORMAT(STR_TO_DATE(erern.`date`, '%Y%m%d'), '%Y.%m.%d') AS ReqDate");
                sb.AppendLine($"              , DATE_FORMAT(erern.regDate, '%Y.%m.%d %T') AS SendDate");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '미발송' WHEN erern.pushYn = 'Y' THEN '-' END AS SendStatus");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '카카오톡' WHEN erern.pushYn = 'Y' THEN 'App Push' END AS SendType");
                sb.AppendLine($"              , CASE");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.ptntNm IS NULL OR erern.ptntNm = '' OR erern.ptntNm = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.phone IS NULL OR erern.phone = '' OR erern.phone = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.examPushSet IS NULL OR erern.examPushSet NOT IN (1, 2, 4)) THEN '검사결과 알림 서비스 사용 안함 상태입니다. 병원정보관리>헬로100설정에서 설정할 수 있습니다.'");
                sb.AppendLine($"                END AS Message");
                sb.AppendLine($"              , HEX(erern.notificationId) AS NotificationId");
                sb.AppendLine($"           FROM hello100_api.eghis_recept_examination_result_notification erern");
                sb.AppendLine($"           LEFT JOIN hello100_api.eghis_recept_examination_result erer ON erer.hospNo = erern.hospNo AND erer.receptNo = erern.receptNo AND erer.date = erern.date");
                sb.AppendLine($"          WHERE NOT EXISTS ( SELECT 'Y'");
                sb.AppendLine($"                               FROM tb_eghis_hosp_receipt_info teri");
                sb.AppendLine($"                              WHERE teri.hosp_no = erern.hospNo");
                sb.AppendLine($"                                AND teri.recept_no = erern.receptNo )");
                sb.AppendLine($"            {(conditions[1].Any() ? $"AND {string.Join($"{Environment.NewLine}            AND ", conditions[1])}" : string.Empty)}");
                sb.AppendLine($"       ) a;");

                using var connection = _connection.CreateConnection();
                var queryResult = (await connection.QueryAsync<SearchExaminationResultAlimtalkHistoriesRow>(sb.ToString(), parameters)).ToList();

                var resultItem = queryResult.Adapt<List<SearchExaminationResultAlimtalkHistoryReadModel>>();

                var result = new SearchExaminationResultAlimtalkHistoriesReadModel
                {
                    List = resultItem
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SearchExaminationResultAlimtalkHistoriesAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        // SearchExaminationResultAlimtalkHistoriesAsync() 과 쿼리는 동일 Page Offset만 다른데. 추후 합치는 방안 고려
        // Key값 하드코딩된 부분도 추후 개선 필요
        public async Task<List<GetExaminationResultAlimtalkHistoryForExportReadModel>> GetExaminationResultAlimtalkHistoryForExportAsync(ExportExaminationResultAlimtalkHistoriesExcelQuery req, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetExaminationResultAlimtalkHistoryForExportAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("@SearchKeyword", req.SearchKeyword ?? string.Empty, DbType.String);
                parameters.Add("@FromDt", Convert.ToDateTime(req.FromDate).ToString("yyyyMMdd"), DbType.String);
                parameters.Add("@ToDt", Convert.ToDateTime(req.ToDate).ToString("yyyyMMdd"), DbType.String);
                parameters.Add("@NameKey", "dcc2b29aaa9f271d", DbType.String); // AES256Key_Email_Name
                parameters.Add("@DecKey", "08a0d3a6ec32e85e", DbType.String); // AES256Key
                parameters.Add("@HospNo", req.HospNo, DbType.String);
                //parameters.Add("@HospNo", "10350072", DbType.String);

                string dateColumn = "erern.regDate";

                if (req.SearchDateType == 1) dateColumn = "erern.regDate";
                else if (req.SearchDateType == 2) dateColumn = @"erern.`date`";

                var conditions = new List<List<string>>()
                {
                    new List<string>()
                    {
                        "erern.hospNo = @HospNo",
                        $"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt"
                    },
                    new List<string>()
                    {
                        "erern.hospNo = @HospNo",
                        $"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt"
                    }
                };

                // 검색어
                if (string.IsNullOrWhiteSpace(req.SearchKeyword) == false)
                {
                    conditions[0].Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(tm.name), @NameKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                    conditions[1].Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(erern.ptntNm), @DecKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("SET block_encryption_mode = 'aes-128-cbc';");

                sb.AppendLine($"SELECT ROW_NUMBER() OVER(ORDER BY a.`Date` DESC, a.ReceptNo DESC) AS RowNum, a.*");
                sb.AppendLine($"  FROM (");
                sb.AppendLine(@"         SELECT CONVERT(AES_DECRYPT(FROM_BASE64(tm.name), 'dcc2b29aaa9f271d', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntName");
                sb.AppendLine(@"              , CONVERT(AES_DECRYPT(FROM_BASE64(tm.sex), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntSex");
                sb.AppendLine($"              , erern.`date` AS `Date`");
                sb.AppendLine($"              , erern.receptNo AS ReceptNo");
                sb.AppendLine($"              , DATE_FORMAT(STR_TO_DATE(erern.`date`, '%Y%m%d'), '%Y.%m.%d') AS ReqDate");
                sb.AppendLine($"              , DATE_FORMAT(erern.regDate, '%Y.%m.%d %T') AS SendDate");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '미발송' WHEN erern.pushYn = 'Y' THEN '-' END AS SendStatus");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '카카오톡' WHEN erern.pushYn = 'Y' THEN 'App Push' end AS SendType");
                sb.AppendLine($"              , CASE");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.ptntNm IS NULL OR erern.ptntNm = '' OR erern.ptntNm = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.phone IS NULL OR erern.phone = '' OR erern.phone = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.examPushSet IS NULL OR erern.examPushSet NOT IN (1, 2, 4)) THEN '검사결과 알림 서비스 사용 안함 상태입니다. 병원정보관리>헬로100설정에서 설정할 수 있습니다.'");
                sb.AppendLine($"                END AS Message");
                sb.AppendLine($"              , HEX(erern.notificationId) as NotificationId");
                sb.AppendLine($"           FROM hello100_api.eghis_recept_examination_result_notification erern");
                sb.AppendLine($"           LEFT JOIN hello100_api.eghis_recept_examination_result erer ON erer.hospNo = erern.hospNo AND erer.receptNo = erern.receptNo AND erer.date = erern.date");
                sb.AppendLine($"          INNER JOIN tb_eghis_hosp_receipt_info teri ON teri.hosp_no = erern.hospNo AND teri.recept_no = erern.receptNo");
                sb.AppendLine($"          INNER JOIN tb_member tm on tm.uid = teri.uid AND tm.mid = teri.mid");
                sb.AppendLine($"          WHERE {(conditions[0].Any() ? $"{string.Join($"{Environment.NewLine}            AND ", conditions[0])}" : string.Empty)}");
                sb.AppendLine($"         UNION ALL");
                sb.AppendLine(@"         SELECT CONVERT(AES_DECRYPT(FROM_BASE64(erern.ptntNm), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntName");
                sb.AppendLine($"              , '-' AS PtntSex");
                sb.AppendLine($"              , erern.`date` AS `Date`");
                sb.AppendLine($"              , erern.receptNo AS ReceptNo");
                sb.AppendLine($"              , DATE_FORMAT(STR_TO_DATE(erern.`date`, '%Y%m%d'), '%Y.%m.%d') AS ReqDate");
                sb.AppendLine($"              , DATE_FORMAT(erern.regDate, '%Y.%m.%d %T') AS SendDate");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '미발송' WHEN erern.pushYn = 'Y' THEN '-' END AS SendStatus");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '카카오톡' WHEN erern.pushYn = 'Y' THEN 'App Push' END AS SendType");
                sb.AppendLine($"              , CASE");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.ptntNm IS NULL OR erern.ptntNm = '' OR erern.ptntNm = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.phone IS NULL OR erern.phone = '' OR erern.phone = '+0K2HSgMT6X+z7YDJry9YQ==') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.examPushSet IS NULL OR erern.examPushSet NOT IN (1, 2, 4)) THEN '검사결과 알림 서비스 사용 안함 상태입니다. 병원정보관리>헬로100설정에서 설정할 수 있습니다.'");
                sb.AppendLine($"                END AS Message");
                sb.AppendLine($"              , HEX(erern.notificationId) AS NotificationId");
                sb.AppendLine($"           FROM hello100_api.eghis_recept_examination_result_notification erern");
                sb.AppendLine($"           LEFT JOIN hello100_api.eghis_recept_examination_result erer ON erer.hospNo = erern.hospNo AND erer.receptNo = erern.receptNo AND erer.date = erern.date");
                sb.AppendLine($"          WHERE NOT EXISTS ( SELECT 'Y'");
                sb.AppendLine($"                               FROM tb_eghis_hosp_receipt_info teri");
                sb.AppendLine($"                              WHERE teri.hosp_no = erern.hospNo");
                sb.AppendLine($"                                AND teri.recept_no = erern.receptNo )");
                sb.AppendLine($"            {(conditions[1].Any() ? $"AND {string.Join($"{Environment.NewLine}            AND ", conditions[1])}" : string.Empty)}");
                sb.AppendLine($"       ) a;");

                using var connection = _connection.CreateConnection();
                var queryResult = (await connection.QueryAsync<GetExaminationResultAlimtalkHistoryForExportRow>(sb.ToString(), parameters)).ToList();

                var result = queryResult.Adapt<List<GetExaminationResultAlimtalkHistoryForExportReadModel>>();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetExaminationResultAlimtalkHistoryForExportAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<TbKakaoMsgJoinEntity?> FindAlimtalkServiceApplicationAsync(DbSession db, string hospNo, string hospKey, string tmpType, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("HospNo", hospNo, DbType.String);
            parameters.Add("HospKey", hospKey, DbType.String);
            parameters.Add("TmpType", tmpType, DbType.String);

            var query = @"
                SELECT doct_nm,
                       doct_tel
                  FROM hello100.tb_kakao_msg_join
                 WHERE hosp_no = @HospNo
	               AND hosp_key = @HospKey
	               AND tmp_type = @TmpType
            ";

            var result = await db.QueryFirstOrDefaultAsync<TbKakaoMsgJoinEntity>(query, parameters, ct, _logger);

            return result;
        }

        public async Task<List<GetHospitalServiceUsageStatusResultItemByServiceUnit>> GetServiceUnitReceptionStatusAsync(
            DbSession db, string? fromDate, string? toDate, string? searchChartType, int searchType, string? searchKeyword, string qrCheckYn, 
            string todayRegistrationYn, string appointmentYn, string telemedicineYn, string excludeTestHospitalsYn, CancellationToken ct)
        {
            var fromDt = string.IsNullOrWhiteSpace(fromDate) ? string.Empty : fromDate.Replace("-", "");
            var toDt = string.IsNullOrWhiteSpace(toDate) ? string.Empty : toDate.Replace("-", "");

            #region ParamSet
            var chkGroup = new Dictionary<string, string>
            {
                { "'RC'", qrCheckYn },  //QR 접수 
                { "'TR'", todayRegistrationYn },  //오늘 접수
                { "'RS'", appointmentYn },  //진료 예약
                { "'NR'", telemedicineYn }   //비대면 진료
            };

            List<string> serviceChkList = new List<string>();

            foreach (var x in chkGroup)
            {
                if (x.Value == "Y")
                {
                    serviceChkList.Add(x.Key);
                }
            }

            string inClause = "";

            if (serviceChkList.Count > 0)
            {
                inClause = string.Join(", ", serviceChkList);
            }
            #endregion

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT tc.cm_name               ReceptTypeNm,  ");
            sb.AppendLine("       tc.cm_cd                 ReceptType,    ");
            sb.AppendLine("       IFNULL(x.WaitingCount , 0) WaitingCount, ");
            sb.AppendLine("       IFNULL(x.ReceptionCount , 0) ReceptionCount, ");
            sb.AppendLine("       IFNULL(x.ReceptionFailedCount , 0) ReceptionFailedCount, ");
            sb.AppendLine("       IFNULL(x.ReceptionCanceledCount , 0) ReceptionCanceledCount, ");
            sb.AppendLine("       IFNULL(x.TreatmentCompletedCount , 0) TreatmentCompletedCount, ");
            sb.AppendLine("       IFNULL(x.TotalReceptionCount, 0)         TotalReceptionCount            ");
            sb.AppendLine("  FROM hello100.tb_common tc	                  ");
            sb.AppendLine("  LEFT JOIN ( SELECT recept_type receptType,   ");
            sb.AppendLine("                     SUM(CASE WHEN ptnt_state IN(3, 2) THEN cnt ELSE 0 END) WaitingCount, "); // 접수대기
            sb.AppendLine("                     SUM(CASE WHEN ptnt_state =   1 THEN cnt ELSE 0 END) ReceptionCount, "); // 접수완료
            sb.AppendLine("                     SUM(CASE WHEN ptnt_state =   8 THEN cnt ELSE 0 END) ReceptionCanceledCount, "); // 취소
            sb.AppendLine("                     SUM(CASE WHEN ptnt_state =   7 THEN cnt ELSE 0 END) ReceptionFailedCount, "); // 실패
            sb.AppendLine("                     SUM(CASE WHEN ptnt_state >=  9 THEN cnt ELSE 0 END) TreatmentCompletedCount, "); // 진료완료
            sb.AppendLine("                     SUM(cnt)                                            TotalReceptionCount         "); // 총계
            sb.AppendLine("                FROM ( SELECT tehri.recept_type,                                     ");
            sb.AppendLine("                              tehri.ptnt_state,                                      ");
            sb.AppendLine("                              COUNT(1) AS cnt                                        ");
            sb.AppendLine("                         FROM hello100.tb_eghis_hosp_receipt_info tehri              ");
            sb.AppendLine("                         INNER JOIN hello100.tb_eghis_hosp_info tehi                 ");
            sb.AppendLine("                            ON tehri.hosp_no = tehi.hosp_no                          ");
            sb.AppendLine("                         INNER JOIN hello100.tb_hospital_info thi                    ");
            sb.AppendLine("                            ON tehi.hosp_key = thi.hosp_key                          ");

            if (excludeTestHospitalsYn == "Y")
            {
                sb.AppendLine("                           AND thi.is_test = 0                                       ");
            }

            sb.AppendLine("                        WHERE 1 = 1                                                  ");

            if (string.IsNullOrWhiteSpace(fromDt) == false)
            {
                sb.AppendLine("                          AND tehri.req_date BETWEEN '" + fromDt + "' AND '" + toDt + "'");
            }

            if (serviceChkList.Count > 0)
            {
                sb.AppendLine("                          AND tehri.recept_type IN (" + inClause + ")");
            }

            if (string.IsNullOrWhiteSpace(searchKeyword) == false)
            {
                // 병원명 검색
                if (searchType == 1)
                {
                    sb.AppendLine("                          AND tehri.hosp_no IN ( SELECT hosp_no FROM hello100.vm_eghis_hospitals vh WHERE vh.name LIKE '%" + searchKeyword + "%' )");
                }
                else
                {
                    //요양기관 검색
                    sb.AppendLine("                          AND tehri.hosp_no LIKE '%" + searchKeyword + "%'");
                }
            }

            if (string.IsNullOrWhiteSpace(searchChartType) == false)
            {
                sb.AppendLine("                          AND tehi.chart_type = '" + searchChartType + "'");
            }

            sb.AppendLine("                       GROUP BY tehri.recept_type, tehri.ptnt_state ) x              ");
            sb.AppendLine("                LEFT JOIN hello100.tb_common tc 	                                    ");
            sb.AppendLine("                  ON tc.cls_cd = '16' AND tc.cm_cd = x.recept_type                   ");
            sb.AppendLine("              GROUP BY recept_type ) x 	                                            ");
            sb.AppendLine("    ON tc.cls_cd = '16' AND tc.cm_cd = x.receptType	                                ");
            sb.AppendLine(" WHERE tc.cls_cd = '16'                                                              ");
            sb.AppendLine("ORDER BY tc.cm_seq;                                                                  ");

            var result = (await db.QueryAsync<GetHospitalServiceUsageStatusResultItemByServiceUnit>(sb.ToString(), ct: ct, logger: _logger)).ToList();

            return result;
        }

        public async Task<ListResult<GetHospitalServiceUsageStatusResultItemByHospitalUnit>> GetHospitalUnitReceptionStatusAsync(
            DbSession db, int pageNo, int pageSize, string? fromDate, string? toDate, string? searchChartType, int searchType, string? searchKeyword, string qrCheckYn,
            string todayRegistrationYn, string appointmentYn, string telemedicineYn, string excludeTestHospitalsYn, CancellationToken ct)
        {
            var fromDt = string.IsNullOrWhiteSpace(fromDate) ? string.Empty : fromDate.Replace("-", "");
            var toDt = string.IsNullOrWhiteSpace(toDate) ? string.Empty : toDate.Replace("-", "");

            var parameters = new DynamicParameters();
            parameters.Add("SearchKeyword", searchKeyword, DbType.String);
            parameters.Add("Limit", pageSize, DbType.Int32);
            parameters.Add("OffSet", (pageNo - 1) * pageSize, DbType.Int32);
            parameters.Add("FromDt", fromDt, DbType.String);
            parameters.Add("ToDt", toDt, DbType.String);

            var chkGroup = new Dictionary<string, string>
            {
                { "'RC'", qrCheckYn },  //QR 접수 
                { "'TR'", todayRegistrationYn },  //오늘 접수
                { "'RS'", appointmentYn },  //진료 예약
                { "'NR'", telemedicineYn }   //비대면 진료
            };

            List<string> serviceChkList = new List<string>();

            foreach (var x in chkGroup)
            {
                if (x.Value == AdminBizConstant.StringYn.YES)
                {
                    serviceChkList.Add(x.Key);
                }
            }

            string inClause = "";

            if (serviceChkList.Count > 0)
            {
                inClause = string.Join(", ", serviceChkList);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT cn.hosp_no AS HospNo,");
            sb.AppendLine("       MAX(vh.name) AS HospName,");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state IN (3, 2) THEN cnt ELSE 0 END) AS WaitingCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state =  1 THEN cnt ELSE 0 END) AS ReceptionCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state =  8 THEN cnt ELSE 0 END) AS ReceptionCanceledCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state =  7 THEN cnt ELSE 0 END) AS ReceptionFailedCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state >= 9 THEN cnt ELSE 0 END) AS TreatmentCompletedCount  	");
            sb.AppendLine("  FROM ( SELECT ti.hosp_no,                                                          ");
            sb.AppendLine("                ti.recept_type,                                                      ");
            sb.AppendLine("                ti.ptnt_state,                                                       ");
            sb.AppendLine("                COUNT(1) cnt                                                         ");
            sb.AppendLine("           FROM hello100.tb_eghis_hosp_receipt_info ti                               ");
            sb.AppendLine("                         INNER JOIN hello100.tb_eghis_hosp_info tehi                 ");
            sb.AppendLine("                            ON ti.hosp_no = tehi.hosp_no                             ");
            sb.AppendLine("                         INNER JOIN hello100.tb_hospital_info thi                    ");
            sb.AppendLine("                            ON tehi.hosp_key = thi.hosp_key                          ");

            if (excludeTestHospitalsYn == AdminBizConstant.StringYn.YES)
            {
                sb.AppendLine("                           AND thi.is_test = 0                                       ");
            }

            sb.AppendLine("          WHERE 1 = 1");

            if (string.IsNullOrWhiteSpace(fromDt) == false)
            {
                sb.AppendLine("            AND ti.req_date BETWEEN @FromDt AND @ToDt");
            }

            if (serviceChkList.Count > 0)
            {
                sb.AppendLine("            AND ti.recept_type in (" + inClause + ")");
            }

            if (string.IsNullOrWhiteSpace(searchKeyword) == false)
            {
                //병원명 검색
                if (searchType == 1)
                {
                    sb.AppendLine("            AND ti.hosp_no IN ( SELECT hosp_no FROM hello100.vm_eghis_hospitals vh WHERE vh.name LIKE CONCAT('%', @SearchKeyword, '%') )");
                }
                else
                {
                    //요양기관 검색
                    sb.AppendLine("            AND ti.hosp_no LIKE CONCAT('%', @SearchKeyword, '%')");
                }
            }

            if (string.IsNullOrWhiteSpace(searchChartType) == false)
            {
                sb.AppendLine("                          AND tehi.chart_type = '" + searchChartType + "'");
            }

            sb.AppendLine("         GROUP BY ti.hosp_no, ti.recept_type, ti.ptnt_state");
            sb.AppendLine("         ORDER BY ti.hosp_no ) cn                          ");
            sb.AppendLine("  LEFT JOIN hello100.vm_eghis_hospitals vh                 ");
            sb.AppendLine("    ON cn.hosp_no = vh.hosp_no  	                          ");
            sb.AppendLine("GROUP BY HospNo                                            ");
            sb.AppendLine("ORDER BY HospNo                                            ");
            sb.AppendLine("LIMIT @OffSet, @Limit;                                     ");

            var queryResult = (await db.QueryAsync<GetHospitalServiceUsageStatusResultItemByHospitalUnit>(sb.ToString(), parameters, ct, _logger)).ToList();

            var result = new ListResult<GetHospitalServiceUsageStatusResultItemByHospitalUnit>();

            result.Items = queryResult;
            result.TotalCount = queryResult.Count;

            return result;
        }

        // this.GetHospitalUnitReceptionStatusAsync() 과 pagenation 차이. 추후 병합 가능?
        public async Task<List<GetHospitalServiceUsageStatusResultItemByHospitalUnit>> ExportHospitalUnitReceptionStatusExcelAsync(
            DbSession db, string? fromDate, string? toDate, string? searchChartType, int searchType, string? searchKeyword, string qrCheckYn,
            string todayRegistrationYn, string appointmentYn, string telemedicineYn, string excludeTestHospitalsYn, CancellationToken ct)
        {
            var fromDt = string.IsNullOrWhiteSpace(fromDate) ? string.Empty : fromDate.Replace("-", "");
            var toDt = string.IsNullOrWhiteSpace(toDate) ? string.Empty : toDate.Replace("-", "");

            var parameters = new DynamicParameters();
            parameters.Add("SearchKeyword", searchKeyword, DbType.String);
            parameters.Add("FromDt", fromDt, DbType.String);
            parameters.Add("ToDt", toDt, DbType.String);

            var chkGroup = new Dictionary<string, string>
            {
                { "'RC'", qrCheckYn },  //QR 접수 
                { "'TR'", todayRegistrationYn },  //오늘 접수
                { "'RS'", appointmentYn },  //진료 예약
                { "'NR'", telemedicineYn }   //비대면 진료
            };

            List<string> serviceChkList = new List<string>();

            foreach (var x in chkGroup)
            {
                if (x.Value == AdminBizConstant.StringYn.YES)
                {
                    serviceChkList.Add(x.Key);
                }
            }

            string inClause = "";

            if (serviceChkList.Count > 0)
            {
                inClause = string.Join(", ", serviceChkList);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT cn.hosp_no AS HospNo,");
            sb.AppendLine("       MAX(vh.name) AS HospName,");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state IN (3, 2) THEN cnt ELSE 0 END) AS WaitingCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state =  1 THEN cnt ELSE 0 END) AS ReceptionCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state =  8 THEN cnt ELSE 0 END) AS ReceptionCanceledCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state =  7 THEN cnt ELSE 0 END) AS ReceptionFailedCount, 	");
            sb.AppendLine("       SUM(CASE WHEN ptnt_state >= 9 THEN cnt ELSE 0 END) AS TreatmentCompletedCount  	");
            sb.AppendLine("  FROM ( SELECT ti.hosp_no,                                                          ");
            sb.AppendLine("                ti.recept_type,                                                      ");
            sb.AppendLine("                ti.ptnt_state,                                                       ");
            sb.AppendLine("                COUNT(1) cnt                                                         ");
            sb.AppendLine("           FROM hello100.tb_eghis_hosp_receipt_info ti                               ");
            sb.AppendLine("                         INNER JOIN hello100.tb_eghis_hosp_info tehi                 ");
            sb.AppendLine("                            ON ti.hosp_no = tehi.hosp_no                             ");
            sb.AppendLine("                         INNER JOIN hello100.tb_hospital_info thi                    ");
            sb.AppendLine("                            ON tehi.hosp_key = thi.hosp_key                          ");

            if (excludeTestHospitalsYn == AdminBizConstant.StringYn.YES)
            {
                sb.AppendLine("                           AND thi.is_test = 0                                       ");
            }

            sb.AppendLine("          WHERE 1 = 1");

            if (string.IsNullOrWhiteSpace(fromDt) == false)
            {
                sb.AppendLine("            AND ti.req_date BETWEEN @FromDt AND @ToDt");
            }

            if (serviceChkList.Count > 0)
            {
                sb.AppendLine("            AND ti.recept_type in (" + inClause + ")");
            }

            if (string.IsNullOrWhiteSpace(searchKeyword) == false)
            {
                //병원명 검색
                if (searchType == 1)
                {
                    sb.AppendLine("            AND ti.hosp_no IN ( SELECT hosp_no FROM hello100.vm_eghis_hospitals vh WHERE vh.name LIKE CONCAT('%', @SearchKeyword, '%'))");
                }
                else
                {
                    //요양기관 검색
                    sb.AppendLine("            AND ti.hosp_no LIKE CONCAT('%', @SearchKeyword, '%')");
                }
            }

            if (string.IsNullOrWhiteSpace(searchChartType) == false)
            {
                sb.AppendLine("                          AND tehi.chart_type = '" + searchChartType + "'");
            }

            sb.AppendLine("         GROUP BY ti.hosp_no, ti.recept_type, ti.ptnt_state");
            sb.AppendLine("         ORDER BY ti.hosp_no ) cn                          ");
            sb.AppendLine("  LEFT JOIN hello100.vm_eghis_hospitals vh                 ");
            sb.AppendLine("    ON cn.hosp_no = vh.hosp_no  	                          ");
            sb.AppendLine("GROUP BY HospNo                                            ");
            sb.AppendLine("ORDER BY HospNo                                            ");

            var result = (await db.QueryAsync<GetHospitalServiceUsageStatusResultItemByHospitalUnit>(sb.ToString(), parameters, ct, _logger)).ToList();

            return result;
        }

        public async Task<ExportHello100ReceptionStatusExcelResult> ExportHello100ReceptionStatusExcelAsync(
            DbSession db, string fromDate, string toDate, CancellationToken ct)
        {
            var fromDt = fromDate.Replace("-", "");
            var toDt = toDate.Replace("-", "");

            DateTime fromDtToDateTime = DateTime.ParseExact(fromDt, "yyyyMMdd", null);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT a.hosp_no                                                 AS HospNo,                 ");
            sb.AppendLine("       MAX(c.name)                                               AS HospName,               ");
            sb.AppendLine("       COUNT(1)                                                  AS TotalCount,                 ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 3, 1, 0))                           AS ReservationWaitingCount,    ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 2, 1, 0))                           AS WaitingCount,               ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 1, 1, 0))                           AS ReceptionCount,             ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 8, 1, 0))                           AS CanceledCount,              ");
            sb.AppendLine("       SUM(IF(a.ptnt_state >= 9, 1, 0))                          AS CompletedCount,             ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 7, 1, 0))                           AS FailedCount,                ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 3, 1, 0))  AS QrReservationWaitingCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 2, 1, 0))  AS QrWaitingCount,             ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 1, 1, 0))  AS QrReceptionCount,           ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 8, 1, 0))  AS QrCanceledCount,            ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state >= 9, 1, 0)) AS QrCompletedCount,           ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 3, 1, 0))  AS TodayReservationWaitingCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 2, 1, 0))  AS TodayWaitingCount,          ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 1, 1, 0))  AS TodayReceptionCount,        ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 8, 1, 0))  AS TodayCanceledCount,         ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state >= 9, 1, 0)) AS TodayCompletedCount,        ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 3, 1, 0))  AS ReservationReservationWaitingCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 2, 1, 0))  AS ReservationWaitingCountByType,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 1, 1, 0))  AS ReservationReceptionCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 8, 1, 0))  AS ReservationCanceledCount,   ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state >= 9, 1, 0)) AS ReservationCompletedCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 3, 1, 0))  AS NonFaceToFaceReservationWaitingCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 2, 1, 0))  AS NonFaceToFaceWaitingCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 1, 1, 0))  AS NonFaceToFaceReceptionCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 8, 1, 0))  AS NonFaceToFaceCanceledCount, ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state >= 9, 1, 0)) AS NonFaceToFaceCompletedCount ");
            sb.AppendLine("  FROM tb_eghis_hosp_receipt_info a                                                       ");
            sb.AppendLine("  LEFT JOIN tb_eghis_hosp_info b                                                          ");
            sb.AppendLine("    ON a.hosp_no = b.hosp_no                                                              ");
            sb.AppendLine("  LEFT JOIN tb_hospital_info c                                                            ");
            sb.AppendLine("    ON b.hosp_key = c.hosp_key                                                            ");
            sb.AppendLine(" WHERE DATE_FORMAT(FROM_UNIXTIME(a.reg_dt), '%Y%m%d%H%i%s') BETWEEN '" + fromDtToDateTime.AddDays(-1).ToString("yyyyMMdd") + "090000' AND '" + fromDt + "085959'");
            sb.AppendLine("   AND NOT a.hosp_no LIKE '103500__'                                                      ");
            sb.AppendLine("GROUP BY a.hosp_no;                                                                       ");

            sb.AppendLine("SELECT a.hosp_no                                                 AS HospNo,                 ");
            sb.AppendLine("       MAX(c.name)                                               AS HospName,               ");
            sb.AppendLine("       COUNT(1)                                                  AS TotalCount,                 ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 3, 1, 0))                           AS ReservationWaitingCount,    ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 2, 1, 0))                           AS WaitingCount,               ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 1, 1, 0))                           AS ReceptionCount,             ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 8, 1, 0))                           AS CanceledCount,              ");
            sb.AppendLine("       SUM(IF(a.ptnt_state >= 9, 1, 0))                          AS CompletedCount,             ");
            sb.AppendLine("       SUM(IF(a.ptnt_state = 7, 1, 0))                           AS FailedCount,                ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 3, 1, 0))  AS QrReservationWaitingCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 2, 1, 0))  AS QrWaitingCount,             ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 1, 1, 0))  AS QrReceptionCount,           ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state = 8, 1, 0))  AS QrCanceledCount,            ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RC' AND a.ptnt_state >= 9, 1, 0)) AS QrCompletedCount,           ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 3, 1, 0))  AS TodayReservationWaitingCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 2, 1, 0))  AS TodayWaitingCount,          ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 1, 1, 0))  AS TodayReceptionCount,        ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state = 8, 1, 0))  AS TodayCanceledCount,         ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'TR' AND a.ptnt_state >= 9, 1, 0)) AS TodayCompletedCount,        ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 3, 1, 0))  AS ReservationReservationWaitingCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 2, 1, 0))  AS ReservationWaitingCountByType,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 1, 1, 0))  AS ReservationReceptionCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state = 8, 1, 0))  AS ReservationCanceledCount,   ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'RS' AND a.ptnt_state >= 9, 1, 0)) AS ReservationCompletedCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 3, 1, 0))  AS NonFaceToFaceReservationWaitingCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 2, 1, 0))  AS NonFaceToFaceWaitingCount,  ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 1, 1, 0))  AS NonFaceToFaceReceptionCount,");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state = 8, 1, 0))  AS NonFaceToFaceCanceledCount, ");
            sb.AppendLine("       SUM(IF(a.recept_type = 'NR' AND a.ptnt_state >= 9, 1, 0)) AS NonFaceToFaceCompletedCount ");
            sb.AppendLine("  FROM tb_eghis_hosp_receipt_info a                                                       ");
            sb.AppendLine("  LEFT JOIN tb_eghis_hosp_info b                                                          ");
            sb.AppendLine("    ON a.hosp_no = b.hosp_no                                                              ");
            sb.AppendLine("  LEFT JOIN tb_hospital_info c                                                            ");
            sb.AppendLine("    ON b.hosp_key = c.hosp_key                                                            ");
            sb.AppendLine(" WHERE DATE_FORMAT(FROM_UNIXTIME(a.reg_dt), '%Y%m%d%H%i%s') BETWEEN '" + fromDt + "090000' AND '" + toDt + "235959'");
            sb.AppendLine("   AND NOT a.hosp_no LIKE '103500__'                                                      ");
            sb.AppendLine("GROUP BY a.hosp_no;                                                                       ");

            var multi = await db.QueryMultipleAsync(sb.ToString(), ct: ct, logger: _logger);

            var result = new ExportHello100ReceptionStatusExcelResult();

            result.YesterdayItems = (await multi.ReadAsync<ExportHello100ReceptionStatusExcelResultItem>()).ToList();
            result.PeriodItems = (await multi.ReadAsync<ExportHello100ReceptionStatusExcelResultItem>()).ToList();

            return result;
        }
        #endregion
    }
}
