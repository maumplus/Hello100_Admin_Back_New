using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using System.Data;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;
using DocumentFormat.OpenXml.Wordprocessing;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;
using System.Text.Encodings.Web;
using Org.BouncyCastle.Ocsp;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Drawing;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.RequestsManagement
{
    public class RequestsManagementStore : IRequestsManagementStore
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<RequestsManagementStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public RequestsManagementStore(IDbConnectionFactory connection, ILogger<RequestsManagementStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region IREQUESTSMANAGEMENTSTORE IMPLEMENTS METHOD AREA **************************************

        public async Task<ListResult<GetRequestBugsResult>> GetRequestBugsAsync(int pageSize, int pageNum, bool apprYn, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRequestBugsAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("PageSize", pageSize, DbType.Int32);
                parameters.Add("OffSet", (pageNum - 1) * pageSize, DbType.Int32);

                #region == Query ==
                var builder = new SqlBuilder();

                var listTemplate = builder.AddTemplate(@"
                    SELECT ROW_NUMBER() OVER (ORDER BY RegDt ASC) AS RowNum
                        ,   z.*                                             
                       FROM (                                               
                     SELECT a.hp_id			AS HpId                         
	                    ,   a.hosp_key	    AS HospKey                      
	                    ,   d.hosp_no	    AS HospNo                      
	                    ,   b.addr	        AS HospAddr                     
	                    ,	b.name		    AS Name                         
	                    , 	IFNULL(c.name, '') AS ApprName                  
	                    , 	CASE WHEN IFNULL(a.appr_dt, '') = '' THEN 'N' ELSE 'Y' End AS ApprYn 
	                    , 	FROM_UNIXTIME(a.reg_dt, '%Y-%m-%d %H:%i') AS regDt      
	                    , 	FROM_UNIXTIME(a.appr_dt, '%Y-%m-%d %H:%i') AS ApprDt    
                       FROM tb_hospital_proposal_info a     
                      INNER JOIN tb_hospital_info b         
                         ON (b.hosp_key = a.hosp_key)	    
                       LEFT JOIN tb_admin c                 
	                     ON (c.acc_id = a.appr_aid)            
                       LEFT OUTER JOIN tb_eghis_hosp_info d    
	                     ON (d.hosp_key = b.hosp_key)            
                       /**where**/
                       ORDER BY a.reg_dt DESC) z               
                      LIMIT @OffSet, @PageSize;                
                ");

                var countTemplate = builder.AddTemplate(@"
                    SELECT COUNT(*) FROM tb_hospital_proposal_info a
                     INNER JOIN tb_hospital_info b ON (b.hosp_key = a.hosp_key)
                      LEFT JOIN tb_admin c ON (c.acc_id = a.appr_aid)             
                     /**where**/
                ");

                if (apprYn) builder.Where("IFNULL(a.appr_aid,'') = ''");

                var sql = listTemplate.RawSql + countTemplate.RawSql;
                #endregion
                /*
                System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + sql);
                foreach (var paramName in parameters.ParameterNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
                }
                */

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(sql, parameters);

                var queryList = (await multi.ReadAsync<GetRequestBugsResult>()).AsList();
                var totalCount = await multi.ReadSingleAsync<int>();

                var result = new ListResult<GetRequestBugsResult>
                {
                    Items = queryList,
                    TotalCount = totalCount
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRequestBugsAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<GetRequestBugResult?> GetRequestBugAsync(long hpId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRequestBugAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HpId", hpId, DbType.Int64);

                #region == Query ==

                var sql = @"
                     SELECT a.hp_id		AS HpId
	                    , 	a.hosp_key	AS HospKey
	                    , 	d.hosp_no	AS HospNo
	                    , 	c.addr  	AS HospAddr
	                    , 	c.name      AS Name
	                    , 	a.msg		AS Msg
	                    ,	a.uid		AS UId
	                    ,	b.email		AS Email
	                    , 	FROM_UNIXTIME(a.reg_dt, '%Y-%m-%d %H:%i') AS regDt
	                    , 	a.appr_aid	AS ApprAid
	                    , 	FROM_UNIXTIME(a.appr_dt, '%Y-%m-%d %H:%i') AS ApprDt
	                    , 	a.proposal_type	AS ProposalType
	                    ,	(SELECT IFNULL(GROUP_CONCAT(z.cm_name),'') 	
			                    FROM tb_common z
		                      WHERE z.cls_cd = '12'
		  	                     AND FIND_IN_SET(z.cm_cd, a.proposal_type)) AS ProposalName
                      FROM tb_hospital_proposal_info a
                     LEFT JOIN tb_user b
                        ON (b.uid = a.uid)
                     INNER JOIN tb_hospital_info c
                        ON (c.hosp_key = a.hosp_key)
                     LEFT OUTER JOIN tb_eghis_hosp_info d
                        ON (d.hosp_key = c.hosp_key)
                     WHERE a.hp_id = @HpId
                ";
                #endregion
                /*
                System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + sql);
                foreach (var paramName in parameters.ParameterNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
                }
                */

                using var connection = _connection.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<GetRequestBugResult>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRequestBugAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<ListResult<GetRequestUntactsResult>> GetRequestUntactsAsync(
            int pageSize, int pageNum, int searchType, int searchDateType, string? fromDate, string? toDate, string? searchKeyword, List<string> joinState, bool isExcel, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRequestUntactsAsync() Started");

                string paramJoinState = string.Join(",", joinState);
                var parameters = new DynamicParameters();
                parameters.Add("PageSize", pageSize, DbType.Int32);
                parameters.Add("OffSet", (pageNum - 1) * pageSize, DbType.Int32);
                parameters.Add("FromDate", fromDate, DbType.String);
                parameters.Add("ToDate", toDate, DbType.String);
                parameters.Add("SearchKeyword", searchKeyword , DbType.String);
                parameters.Add("JoinState", paramJoinState, DbType.String);

                #region == Query ==
                var builder = new SqlBuilder();

                var listTemplate = builder.AddTemplate(@"
                    select ROW_NUMBER() OVER (ORDER BY x.reg_dt) AS RowNum,
                           y.name as HospNm,
                           x.hosp_key as HospKey,
                           x.hosp_no as HospNo,
                           x.seq as Seq,
                           (select cm_name from hello100.tb_common tc where cls_cd='25' and tc.cm_cd=x.join_state) as JoinStateNm,
                           x.join_state as JoinState,
                           from_unixtime(x.reg_dt, '%Y-%m-%d %H:%i')  as RegDt,
                           case when join_state_mod_dt !=0 then  from_unixtime(join_state_mod_dt, '%Y-%m-%d %H:%i') else '' end as ModDt,
                           doct_nm as DoctNm,
                           doct_tel as DoctTel
                      from tb_eghis_doct_untact_join x
                           inner join hello100.tb_hospital_info y on x.hosp_key = y.hosp_key
                     /**where**/
                     order by x.reg_dt desc
                ");

                var countTemplate = builder.AddTemplate(@"
                    SELECT COUNT(*)
                      FROM tb_eghis_doct_untact_join x 
                           inner join hello100.tb_hospital_info y on x.hosp_key = y.hosp_key
                     /**where**/
                ");

                if (!string.IsNullOrWhiteSpace(fromDate))
                {
                    builder.Where("DATE_FORMAT(FROM_UNIXTIME(x.reg_dt), '%Y-%m-%d') between @FromDate and @ToDate");
                }

                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    switch (searchType)
                    {
                        // 병원명
                        case 1:
                            builder.Where("y.name like concat('%', @SearchKeyword, '%')");
                            break;
                        // 의사명
                        case 2:
                            builder.Where("x.doct_nm like concat('%', @SearchKeyword, '%')");
                            break;
                        // 요양기관번호
                        case 3:
                            builder.Where("x.hosp_no like concat('%', @SearchKeyword, '%')");
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(paramJoinState))
                {
                    builder.Where($"FIND_IN_SET(x.join_state, @JoinState)");
                }

                // 엑셀의 경우 LIMIT 구문을 제거하기 위해 isExcel param을 활용
                // 같은 SQL을 사용하는 여러개의 Store를 만들고 싶지 않아서
                string listSql = listTemplate.RawSql;

                listSql += isExcel ? ";" : " LIMIT @OffSet, @PageSize;";

                var sql = listSql + countTemplate.RawSql;
                #endregion

                System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + sql);
                foreach (var paramName in parameters.ParameterNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
                }

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(sql, parameters);

                var queryList = (await multi.ReadAsync<GetRequestUntactsResult>()).AsList();
                var totalCount = await multi.ReadSingleAsync<int>();

                var result = new ListResult<GetRequestUntactsResult>
                {
                    Items = queryList,
                    TotalCount = totalCount
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRequestUntactsAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }

        }

        public async Task<GetRequestUntactResult?> GetRequestUntactAsync(int seq, string rootUrl, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRequestUntactAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("Seq", seq, DbType.Int32);
                parameters.Add("RootUrl", rootUrl, DbType.String);

                #region == Query ==

                var sql = @"
                    select seq as Seq,
                           x.hosp_no as HospNo,
                           x.hosp_key as HospKey,
                           y.name as HospNm,
                           y.addr as Addr,
                           x.empl_no as EmplNo,
                           x.hosp_tel as HospTel,
                           x.post_cd as PostCd,
                           doct_no as DoctNo,
                           doct_no_type as DoctNoType,
                           (select cm_name from tb_common tc where tc.cls_cd = '23' and tc.cm_cd = x.doct_no_type) as DoctNoTypeNm,
                           doct_license_file_seq as DoctLicenseFileSeq,
                           doct_nm as DoctNm,
                           doct_birthday as DoctBirthday,
                           doct_tel as DoctTel,
                           doct_intro as DoctIntro,
                           doct_file_seq as DoctFileSeq,
                           doct_history as DoctHistory,
                           clinic_time as ClinicTime,
                           clinic_guide as ClinicGuide,
                           account_info_file_seq as AccountInfoFileSeq,
                           join_state as JoinState,
                           from_unixtime(x.reg_dt, '%Y-%m-%d %H:%i') as RegDt ,
                           concat(@RootUrl,(select file_path from tb_file_info tfi where tfi.seq= x.doct_file_seq )) as DoctFilePath,
                           concat(@RootUrl,(select file_path from tb_file_info tfi where tfi.seq= x.business_file_seq )) as BusinessFilePath,
                           concat(@RootUrl,(select file_path from tb_file_info tfi where tfi.seq= x.doct_license_file_seq )) as LicenseFilePath,
                           concat(@RootUrl,(select file_path from tb_file_info tfi where tfi.seq= x.account_info_file_seq )) as AccountFilePath,
                           concat(@RootUrl,(select cm_name from tb_common tc where tc.cls_cd='25' and tc.cm_cd=x.join_state)) as JoinStateNm,
                           x.return_reason as ReturnReason 
                      from tb_eghis_doct_untact_join x
                           left join tb_hospital_info y on x.hosp_key  = y.hosp_key
                     where seq = @Seq
                ";
                #endregion

                System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + sql);
                foreach (var paramName in parameters.ParameterNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
                }

                using var connection = _connection.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<GetRequestUntactResult>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRequestUntactAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }
        #endregion
    }
}
