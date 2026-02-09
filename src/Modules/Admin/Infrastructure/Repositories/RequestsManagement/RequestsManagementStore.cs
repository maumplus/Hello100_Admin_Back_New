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

        #region IVISITPURPOSESTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<ListResult<GetRequestsResult>> GetRequestsAsync(string hospKey, int pageSize, int pageNum, string? apprYn, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRequestsAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospKey", hospKey, DbType.String);
                parameters.Add("Limit", pageSize, DbType.Int32);
                parameters.Add("OffSet", (pageNum - 1) * pageSize, DbType.Int32);
                parameters.Add("ApprYn", apprYn == "Y" ? string.Empty : "N", DbType.String);

                #region == Query ==
                StringBuilder sb = new StringBuilder();

                string sWhere = " hosp_key = @HospKey AND appr_type NOT IN ('KW', 'HO') AND appr_yn LIKE CONCAT('%',@ApprYn, '%')";

                sb.AppendLine(" SELECT 	ROW_NUMBER() OVER (ORDER BY req_dt ASC) AS RowNum    ");
                sb.AppendLine(" 	,	hosp_key		AS HospKey          ");
                sb.AppendLine(" 	,	appr_id		    AS ApprId           ");
                sb.AppendLine(" 	,	appr_type	    AS ApprType         ");
                sb.AppendLine(" 	,	DATA			AS Data             ");
                sb.AppendLine(" 	,	aid			    AS AId              ");
                sb.AppendLine(" 	,	aid_name		AS AIdName          ");
                sb.AppendLine(" 	,	req_aid		    AS ReqAId           ");
                sb.AppendLine(" 	,	appr_yn		    AS ApprYn           ");
                sb.AppendLine(" 	,	from_unixtime(appr_dt, '%Y-%m-%d %H:%i')	AS ApprDt");
                sb.AppendLine(" 	,	from_unixtime(req_dt, '%Y-%m-%d %H:%i')	AS ReqDt");
                sb.AppendLine(" 	,	hosp_cls_cd	    AS HospClsCd        ");
                sb.AppendLine(" 	,	NAME			AS Name             ");
                sb.AppendLine(" 	,	addr			AS Addr             ");
                sb.AppendLine(" 	,	post_cd		    AS PostCd           ");
                sb.AppendLine(" 	,	tel			    AS Tel              ");
                sb.AppendLine(" 	,	site			AS Site             ");
                sb.AppendLine(" 	,	lat			    AS Lat              ");
                sb.AppendLine(" 	,	lng			    AS Lng              ");
                sb.AppendLine(" 	,	from_unixtime(reg_dt, '%Y-%m-%d %H:%i')  AS RegDt");
                sb.AppendLine("    FROM VM_APPROVAL_INFO                    ");
                sb.AppendLine($"   WHERE {sWhere}  ");
                sb.AppendLine("   ORDER BY ReqDt DESC                              ");
                sb.AppendLine("   LIMIT @OffSet, @Limit;");

                sb.AppendLine($" SELECT COUNT(*) FROM VM_APPROVAL_INFO WHERE {sWhere};");
                #endregion
                /*
                System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + sb.ToString());
                foreach (var paramName in parameters.ParameterNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
                }*/

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(sb.ToString(), parameters);

                var queryList = (await multi.ReadAsync<GetRequestsResult>()).AsList();
                var totalCount = await multi.ReadSingleAsync<int>();

                var result = new ListResult<GetRequestsResult>
                {
                    Items = queryList,
                    TotalCount = totalCount
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetRequestsResult() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<ListResult<GetRequestBugsResult>> GetRequestBugsAsync(int pageSize, int pageNum, bool apprYn, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetRequestBugsResult() Started");

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

                System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + sql);
                foreach (var paramName in parameters.ParameterNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
                }

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
                _logger.LogError(e, "GetRequestBugsResult() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<GetRequestBugResult> GetRequestBugAsync(long hpId, CancellationToken token)
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

                System.Diagnostics.Debug.WriteLine("Executing SQL:\n" + sql);
                foreach (var paramName in parameters.ParameterNames)
                {
                    System.Diagnostics.Debug.WriteLine($"Parameter {paramName} = {parameters.Get<object>(paramName)}");
                }

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
        #endregion
    }
}
