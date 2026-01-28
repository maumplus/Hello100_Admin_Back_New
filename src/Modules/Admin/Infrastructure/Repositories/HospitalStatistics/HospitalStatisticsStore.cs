using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Microsoft.Extensions.Logging;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalStatistics
{
    public class HospitalStatisticsStore : IHospitalStatisticsStore
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<HospitalStatisticsStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public HospitalStatisticsStore(IDbConnectionFactory connection, ILogger<HospitalStatisticsStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region IHOSPITALSTATISTICSSTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<List<GetRegistrationStatsByMethodResult>> GetRegistrationStatsByMethodAsync(string hospNo, string year, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRegistrationStatsByMethodAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospNo", hospNo, DbType.String);
                parameters.Add("ReqYear", year, DbType.String);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" select @HospNo as HospNo");
                sb.AppendLine("       , m.MonthNm as MonthNm");
                sb.AppendLine("       , d.QrRecept");
                sb.AppendLine("       , d.QrCancel");
                sb.AppendLine("       , d.Recept");
                sb.AppendLine("       , d.ReceptCancel");
                sb.AppendLine("       , d.Rsrv");
                sb.AppendLine("       , d.RsrvCancel");
                sb.AppendLine("       , d.NonContact");
                sb.AppendLine("       , d.NonContactCancel");
                sb.AppendLine("  from (select @HospNo as HospNo, DATE_FORMAT(`date`, '%m') as MonthNm");
                sb.AppendLine("  		  from hello100_api.vw_get_month_calendar v ");
                sb.AppendLine("		 where left(`date`, 4) = @ReqYear");
                sb.AppendLine("		 group by DATE_FORMAT(`date`, '%m')) m");
                sb.AppendLine("  left join (");
                sb.AppendLine("	 SELECT a.hosp_no AS HospNo                                                                                     ");
                sb.AppendLine("	  	,	DATE_FORMAT(a.req_date, '%m') AS MonthNm                                                                ");
                sb.AppendLine("	  	,	SUM(case when a.recept_type = 'RC' AND a.ptnt_state not in (7, 8) then cnt ELSE 0 END) AS 'QrRecept'             ");
                sb.AppendLine("	  	,	SUM(case when a.recept_type = 'RC' AND a.ptnt_state in (7, 8) then cnt ELSE 0 END) AS 'QrCancel'              ");
                sb.AppendLine("	     ,   SUM(case when a.recept_type = 'TR' AND a.ptnt_state not in (7, 8) then cnt ELSE 0 END) AS 'Recept'               ");
                sb.AppendLine("	  	,	SUM(case when a.recept_type = 'TR' AND a.ptnt_state in (7, 8) then cnt ELSE 0 END) AS 'ReceptCancel'          ");
                sb.AppendLine("	     ,   SUM(case when a.recept_type = 'RS' AND a.ptnt_state not in (7, 8) then cnt ELSE 0 END) AS 'Rsrv'                 ");
                sb.AppendLine("	  	,	SUM(case when a.recept_type = 'RS' AND a.ptnt_state in (7, 8) then cnt ELSE 0 END) AS 'RsrvCancel'            ");
                sb.AppendLine("	     ,   SUM(case when a.recept_type = 'NR' AND a.ptnt_state not in (7, 8) then cnt ELSE 0 END) AS 'NonContact'                 ");
                sb.AppendLine("	  	,	SUM(case when a.recept_type = 'NR' AND a.ptnt_state in (7, 8) then cnt ELSE 0 END) AS 'NonContactCancel'            ");
                sb.AppendLine("	    FROM (SELECT hosp_no                                                                                         ");
                sb.AppendLine("	    			,	req_date                                                                                        ");
                sb.AppendLine("	 			,	serial_no                                                                                       ");
                sb.AppendLine("	 			,	recept_type                                                                                     ");
                sb.AppendLine("	 			,	ptnt_state                                                                                      ");
                sb.AppendLine("	 			,	1 AS cnt                                                                                        ");
                sb.AppendLine("	 		   FROM hello100.tb_eghis_hosp_receipt_info                                                             ");
                sb.AppendLine("	 		  WHERE hosp_no = @HospNo                                                                               ");
                sb.AppendLine("	 		  	AND LEFT(req_date, 4) = @ReqYear) a   ");
                sb.AppendLine("	   INNER JOIN                                                                                                    ");
                sb.AppendLine("	 	  	(SELECT hospNo                                                                                          ");
                sb.AppendLine("	 	  		,	clinicYmd                                                                                       ");
                sb.AppendLine("	 	  		,	serialNo                                                                                        ");
                sb.AppendLine("	 	  	   FROM hello100_api.eghis_recept_over                                                                  ");
                sb.AppendLine("	 	  	  WHERE hospNo = @HospNo                                                                                ");
                sb.AppendLine("	 	  	    AND clinicYmd LIKE CONCAT(@ReqYear,'%')                                                             ");
                sb.AppendLine("	 		UNION ALL                                                                                               ");
                sb.AppendLine("	 		 SELECT hospNo                                                                                          ");
                sb.AppendLine("	 		 	,	rsrvYmd AS clinicYmd                                                                            ");
                sb.AppendLine("	 		 	,	serialNo                                                                                        ");
                sb.AppendLine("	 		   FROM hello100_api.eghis_rsrv_info                                                                    ");
                sb.AppendLine("	 	  	  WHERE hospNo = @HospNo                                                                                ");
                sb.AppendLine("	 	  	    AND rsrvYmd LIKE CONCAT(@ReqYear,'%')) b   ");
                sb.AppendLine("	 		ON (b.hospNo = a.hosp_no AND b.clinicYmd = a.req_date AND b.serialNo = a.serial_no)");
                sb.AppendLine("	   GROUP BY a.hosp_no, DATE_FORMAT(a.req_date, '%m')) d on m.MonthNm = d.MonthNm and m.HospNo = d.HospNo");
                sb.AppendLine("   ORDER BY MonthNm;");
                #endregion

                using var connection = _connection.CreateConnection();
                var result = (await connection.QueryAsync<GetRegistrationStatsByMethodResult>(sb.ToString(), parameters)).ToList();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRegistrationStatsByMethodAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<List<GetRegistrationStatusSummaryResult>> GetRegistrationStatusSummaryAsync(string hospNo, string year, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRegistrationStatusSummaryAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospNo", hospNo, DbType.String);
                parameters.Add("ReqYear", year, DbType.String);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" select @HospNo as HospNo");
                sb.AppendLine("       , m.MonthNm as MonthNm");
                sb.AppendLine("       , d.CreatePtntType");
                sb.AppendLine("       , d.Recept");
                sb.AppendLine("       , d.Cancel");
                sb.AppendLine("  from (select @HospNo as HospNo, DATE_FORMAT(`date`, '%m') as MonthNm");
                sb.AppendLine("  		  from hello100_api.vw_get_month_calendar v ");
                sb.AppendLine("		 where left(`date`, 4) = @ReqYear");
                sb.AppendLine("		 group by DATE_FORMAT(`date`, '%m')) m");
                sb.AppendLine("  left join (");
                sb.AppendLine("	  SELECT a.hosp_no AS HospNo");
                sb.AppendLine("	  	,	DATE_FORMAT(a.req_date, '%m') AS MonthNm");
                sb.AppendLine("	  	,	b.createPtntType AS CreatePtntType");
                sb.AppendLine("	  	,	SUM(case when a.ptnt_state not in (7,8) then cnt ELSE 0 END) AS 'Recept'");
                sb.AppendLine("	  	,	SUM(case when a.ptnt_state in (7,8) then cnt ELSE 0 END) AS 'Cancel'");
                sb.AppendLine("	    FROM (SELECT hosp_no");
                sb.AppendLine("	    		,	req_date");
                sb.AppendLine("	 			,	serial_no");
                sb.AppendLine("	 			,	ptnt_state");
                sb.AppendLine("	 			,	1 AS cnt");
                sb.AppendLine("	 		   FROM hello100.tb_eghis_hosp_receipt_info");
                sb.AppendLine("	 		  WHERE hosp_no = @HospNo");
                sb.AppendLine("	 		  	AND recept_type <> 'CS'");
                sb.AppendLine("	 		  	AND LEFT(req_date, 4) = @ReqYear) a");
                sb.AppendLine("	   INNER JOIN");
                sb.AppendLine("	 	  	(SELECT hospNo");
                sb.AppendLine("	 	  		,	clinicYmd");
                sb.AppendLine("	 	  		,	serialNo");
                sb.AppendLine("	 	  		,	1 AS createPtntType");
                sb.AppendLine("	 	  	   FROM hello100_api.eghis_recept_over");
                sb.AppendLine("	 	  	  WHERE hospNo = @HospNo");
                sb.AppendLine("	 	  	    AND clinicYmd LIKE CONCAT(@ReqYear,'%')");
                sb.AppendLine("	 		UNION ALL");
                sb.AppendLine("	 		 SELECT hospNo");
                sb.AppendLine("	 		 	,	rsrvYmd AS clinicYmd");
                sb.AppendLine("	 		 	,	serialNo");
                sb.AppendLine("	 	  		,	1 AS createPtntType");
                sb.AppendLine("	 		   FROM hello100_api.eghis_rsrv_info");
                sb.AppendLine("	 	  	  WHERE hospNo = @HospNo");
                sb.AppendLine("	 	  	    AND rsrvYmd LIKE CONCAT(@ReqYear,'%')) b");
                sb.AppendLine("	 		ON (b.hospNo = a.hosp_no AND b.clinicYmd = a.req_date AND b.serialNo = a.serial_no)");
                sb.AppendLine("	   GROUP BY a.hosp_no, DATE_FORMAT(a.req_date, '%m'), b.createPtntType) d on m.MonthNm = d.MonthNm and m.HospNo = d.HospNo");
                sb.AppendLine("   ORDER BY MonthNm ;");
                #endregion

                using var connection = _connection.CreateConnection();
                var result = (await connection.QueryAsync<GetRegistrationStatusSummaryResult>(sb.ToString(), parameters)).ToList();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRegistrationStatusSummaryAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<List<GetRegistrationStatsByVisitPurposeResult>> GetRegistrationStatsByVisitPurposeAsync(string hospNo, string yearMonth, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRegistrationStatsByVisitPurposeAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospNo", hospNo, DbType.String);
                parameters.Add("ReqYm", yearMonth, DbType.String);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" SELECT a.hosp_no       AS HospNo                                        ");
                sb.AppendLine("    ,   a.visit_purpose AS VisitPurpose                                  ");
                sb.AppendLine("    ,   a.visit_purpose AS PurposeNm                                     ");
                sb.AppendLine("    ,   SUM(case when a.ptnt_state <> 8 then 1 ELSE 0 END) AS 'Recept'   ");
                sb.AppendLine("    ,   SUM(case when a.ptnt_state = 8 then 1 ELSE 0 END) AS 'Cancel'    ");
                sb.AppendLine("   FROM hello100.tb_eghis_hosp_receipt_info a                            ");
                sb.AppendLine("   INNER JOIN hello100_api.eghis_recept_over b                           ");
                sb.AppendLine("   	ON b.hospNo = a.hosp_no                                             ");
                sb.AppendLine("   	AND b.receptNo = a.recept_no                                        ");
                sb.AppendLine("  WHERE a.hosp_no = @HospNo                                              ");
                sb.AppendLine("    AND a.req_date LIKE CONCAT(@ReqYm, '%')                            ");
                sb.AppendLine("    AND a.visit_purpose is not null                            ");
                sb.AppendLine("  GROUP BY a.visit_purpose                                               ");
                #endregion

                using var connection = _connection.CreateConnection();
                var result = (await connection.QueryAsync<GetRegistrationStatsByVisitPurposeResult>(sb.ToString(), parameters)).ToList();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRegistrationStatsByVisitPurposeAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }
        #endregion
    }
}
