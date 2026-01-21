using Dapper;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
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
        public async Task<SearchUntactMedicalHistoriesReadModel?> SearchUntactMedicalHistoriesAsync(SearchUntactMedicalHistoriesQuery req, CancellationToken token)
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

                if (multi != null)
                {
                    var tempResult = new SearchUntactMedicalHistoriesRow();

                    tempResult.DetailList = (await multi.ReadAsync<SearchUntactMedicalHistoryDetailRow>()).ToList();

                    tempResult.TotalCount = await multi.ReadSingleAsync<int>();
                    tempResult.TotalRsrv = await multi.ReadSingleAsync<int>();
                    tempResult.TotalClinicEnd = await multi.ReadSingleAsync<int>();
                    tempResult.TotalClinicFail = await multi.ReadSingleAsync<int>();
                    tempResult.TotalClinicCancel = await multi.ReadSingleAsync<int>();
                    tempResult.TotalSuccessAmt = await multi.ReadSingleAsync<int>();
                    tempResult.TotalProgressAmt = await multi.ReadSingleAsync<int>();
                    tempResult.TotalFailAmt = await multi.ReadSingleAsync<int>();
                    tempResult.TotalSumAmt = await multi.ReadSingleAsync<int>();

                    var result = tempResult.Adapt<SearchUntactMedicalHistoriesReadModel>();

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

                var test = sb.ToString();

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
                _logger.LogInformation("SearchDiagnosticTestResultAlimtalkSendHistoriesAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("@SearchKeyword", req.SearchKeyword ?? string.Empty, DbType.String);
                parameters.Add("@Limit", req.PageSize, DbType.Int32);
                parameters.Add("@OffSet", (req.PageNo - 1) * req.PageSize, DbType.Int32);
                parameters.Add("@FromDt", Convert.ToDateTime(req.FromDate).ToString("yyyyMMdd"), DbType.String);
                parameters.Add("@ToDt", Convert.ToDateTime(req.ToDate).ToString("yyyyMMdd"), DbType.String);
                parameters.Add("@NameKey", "dcc2b29aaa9f271d", DbType.String); // AES256Key_Email_Name
                parameters.Add("@DecKey", "08a0d3a6ec32e85e", DbType.String); // AES256Key
                parameters.Add("@HospNo", req.HospNo, DbType.String);

                #region QUREY
                StringBuilder sb = new StringBuilder();

                var conditions1 = new List<string>();
                var conditions2 = new List<string>();

                // 기본 조건
                conditions1.Add("erern.hospNo = @HospNo");
                conditions2.Add("erern.hospNo = @HospNo");

                string dateColumn = "erern.regDate";

                if (req.SearchDateType == 1) dateColumn = "erern.regDate";
                else if (req.SearchDateType == 2) dateColumn = @"erern.`date`";

                conditions1.Add($"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt");
                conditions2.Add($"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt");

                // 검색어
                if (!string.IsNullOrEmpty(req.SearchKeyword))
                {
                    conditions1.Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(tm.name), @NameKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                    conditions2.Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(erer.ptntNm), @DecKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                }

                sb.AppendLine("SET block_encryption_mode = 'aes-128-cbc';");

                sb.AppendLine($"SELECT ROW_NUMBER() OVER(ORDER BY a.SendDate DESC) AS RowNum, a.*");
                sb.AppendLine($"  FROM (");
                sb.AppendLine(@"         SELECT CONVERT(AES_DECRYPT(FROM_BASE64(erern.ptntNm), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntName");
                sb.AppendLine(@"              , CONVERT(AES_DECRYPT(FROM_BASE64(tm.sex), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntSex");
                sb.AppendLine($"              , erern.`date` AS `Date`");
                sb.AppendLine($"              , erern.receptNo AS ReceptNo");
                sb.AppendLine($"              , DATE_FORMAT(STR_TO_DATE(erern.`date`, '%Y%m%d'), '%Y.%m.%d') AS ReqDate");
                sb.AppendLine($"              , DATE_FORMAT(erern.regDate, '%Y.%m.%d %T') AS SendDate");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '미발송' WHEN erern.pushYn = 'Y' THEN '-' END AS SendStatus");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '카카오톡' WHEN erern.pushYn = 'Y' THEN 'App Push' end AS SendType");
                sb.AppendLine($"              , CASE");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.ptntNm IS NULL OR erern.ptntNm = '') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.phone IS NULL OR erern.phone = '') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.examPushSet IS NULL OR erern.examPushSet NOT IN (1, 2, 4)) THEN '검사결과 알림 서비스 사용 안함 상태입니다. 병원정보관리>헬로100설정에서 설정할 수 있습니다.'");
                sb.AppendLine($"                END AS Message");
                sb.AppendLine($"              , HEX(erern.notificationId) AS NotificationId");
                sb.AppendLine($"           FROM hello100_api.eghis_recept_examination_result_notification erern");
                sb.AppendLine($"          INNER JOIN tb_eghis_hosp_receipt_info teri ON teri.hosp_no = erern.hospNo AND teri.recept_no = erern.receptNo");
                sb.AppendLine($"          INNER JOIN tb_member tm on tm.uid = teri.uid AND tm.mid = teri.mid");
                sb.AppendLine($"          WHERE {(conditions1.Any() ? string.Join($"{Environment.NewLine}            AND ", conditions1) : string.Empty)}");
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
                sb.AppendLine($"            AND {(conditions2.Any() ? string.Join($"{Environment.NewLine}            AND ", conditions2) : string.Empty)}");
                sb.AppendLine($"       ) a LIMIT @OffSet, @Limit;");
                #endregion

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
                _logger.LogError(e, "SearchDiagnosticTestResultAlimtalkSendHistoriesAsync() Error");
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

                #region QUREY
                StringBuilder sb = new StringBuilder();

                var conditions1 = new List<string>();
                var conditions2 = new List<string>();

                // 기본 조건
                conditions1.Add("erern.hospNo = @HospNo");
                conditions2.Add("erern.hospNo = @HospNo");

                string dateColumn = "erern.regDate";

                if (req.SearchDateType == 1) dateColumn = "erern.regDate";
                else if (req.SearchDateType == 2) dateColumn = @"erern.`date`";

                conditions1.Add($"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt");
                conditions2.Add($"DATE_FORMAT({dateColumn}, '%Y%m%d') BETWEEN @FromDt AND @ToDt");

                // 검색어
                if (!string.IsNullOrEmpty(req.SearchKeyword))
                {
                    conditions1.Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(tm.name), @NameKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                    conditions2.Add(@"CONVERT(AES_DECRYPT(FROM_BASE64(erer.ptntNm), @DecKey, '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) LIKE CONCAT('%', @SearchKeyword, '%')");
                }

                sb.AppendLine("SET block_encryption_mode = 'aes-128-cbc';");

                sb.AppendLine($"SELECT ROW_NUMBER() OVER(ORDER BY a.SendDate DESC) AS RowNum, a.*");
                sb.AppendLine($"  FROM (");
                sb.AppendLine(@"         SELECT CONVERT(AES_DECRYPT(FROM_BASE64(erern.ptntNm), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntName");
                sb.AppendLine(@"              , CONVERT(AES_DECRYPT(FROM_BASE64(tm.sex), '08a0d3a6ec32e85e', '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8mb4) AS PtntSex");
                sb.AppendLine($"              , erern.`date` AS `Date`");
                sb.AppendLine($"              , erern.receptNo AS ReceptNo");
                sb.AppendLine($"              , DATE_FORMAT(STR_TO_DATE(erern.`date`, '%Y%m%d'), '%Y.%m.%d') AS ReqDate");
                sb.AppendLine($"              , DATE_FORMAT(erern.regDate, '%Y.%m.%d %T') AS SendDate");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '미발송' WHEN erern.pushYn = 'Y' THEN '-' END AS SendStatus");
                sb.AppendLine($"              , CASE WHEN erern.talkYn = 'Y' THEN '카카오톡' WHEN erern.pushYn = 'Y' THEN 'App Push' end AS SendType");
                sb.AppendLine($"              , CASE");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.ptntNm IS NULL OR erern.ptntNm = '') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.phone IS NULL OR erern.phone = '') THEN '전자차트에 등록된 환자명 혹은 전화번호가 없습니다.'");
                sb.AppendLine($"                     WHEN erern.talkYn = 'Y' AND (erern.examPushSet IS NULL OR erern.examPushSet NOT IN (1, 2, 4)) THEN '검사결과 알림 서비스 사용 안함 상태입니다. 병원정보관리>헬로100설정에서 설정할 수 있습니다.'");
                sb.AppendLine($"                END AS Message");
                sb.AppendLine($"              , HEX(erern.notificationId) AS NotificationId");
                sb.AppendLine($"           FROM hello100_api.eghis_recept_examination_result_notification erern");
                sb.AppendLine($"          INNER JOIN tb_eghis_hosp_receipt_info teri ON teri.hosp_no = erern.hospNo AND teri.recept_no = erern.receptNo");
                sb.AppendLine($"          INNER JOIN tb_member tm on tm.uid = teri.uid AND tm.mid = teri.mid");
                sb.AppendLine($"          WHERE {(conditions1.Any() ? string.Join($"{Environment.NewLine}            AND ", conditions1) : string.Empty)}");
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
                sb.AppendLine($"            AND {(conditions2.Any() ? string.Join($"{Environment.NewLine}            AND ", conditions2) : string.Empty)}");
                sb.AppendLine($"       ) a;");
                #endregion

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
        #endregion
    }
}
