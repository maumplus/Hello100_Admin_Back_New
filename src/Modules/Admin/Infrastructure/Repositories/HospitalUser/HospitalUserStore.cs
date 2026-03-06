using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using System.Data;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalUser
{
    public class HospitalUserStore : IHospitalUserStore
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<HospitalUserStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public HospitalUserStore(IDbConnectionFactory connection, ILogger<HospitalUserStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region IHOSPITALUSERSTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<ListResult<SearchHospitalUsersResult>> SearchHospitalUsersAsync(
            DbSession db, int pageNo, int pageSize, string? fromDt, string? toDt, int keywordSearchType, string? searchKeyword, CancellationToken token)
        {
            var fromDate = string.IsNullOrEmpty(fromDt) ? null : Convert.ToDateTime(fromDt).ToString("yyyyMMdd");
            var toDate = string.IsNullOrEmpty(toDt) ? null : Convert.ToDateTime(toDt).ToString("yyyyMMdd");

            var parameters = new DynamicParameters();
            parameters.Add("SearchKeyword", searchKeyword ?? string.Empty, DbType.String);
            parameters.Add("PageSize", pageSize, DbType.Int32);
            parameters.Add("OffSet", (pageNo - 1) * pageSize, DbType.Int32);
            parameters.Add("FromDt", fromDate, DbType.String);
            parameters.Add("ToDt", toDate, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
               
            sb.AppendLine("SET block_encryption_mode = 'aes-128-cbc';");

            sb.AppendLine("                    SELECT A.uid AS UId,");
            sb.AppendLine("                           B.mid AS MId,");
            sb.AppendLine("                           B.name AS Name,");
            sb.AppendLine("                           A.sns_id AS SnsId,");
            sb.AppendLine("                           A.email AS Email,");
            sb.AppendLine("                           A.pwd AS Pwd,");
            sb.AppendLine("                           B.phone AS Phone,");
            sb.AppendLine("                           B.birthday AS BirthDay,");
            sb.AppendLine("                           B.sex AS Sex,");
            sb.AppendLine("                           IFNULL(B.photo, '') AS photo,");
            sb.AppendLine("                           B.userYn AS UserYn,");
            sb.AppendLine("                           A.login_type AS LoginType,");
            sb.AppendLine("                           DATE_FORMAT(FROM_UNIXTIME(A.reg_dt), '%Y-%m-%d %H:%i:%s') AS RegDt,");
            sb.AppendLine("                           ( SELECT DATE_FORMAT(FROM_UNIXTIME(auth_dt), '%Y-%m-%d %H:%i:%s')");
            sb.AppendLine("                               FROM tb_self_auth_log");
            sb.AppendLine("                              WHERE said = A.said ) AS AuthDt");
            sb.AppendLine("                      FROM tb_user A");
            sb.AppendLine("                     INNER JOIN tb_member B ");
            sb.AppendLine("                             ON A.uid = B.uid ");
            sb.AppendLine("                            AND B.userYn = 'Y'");
            sb.AppendLine("                     WHERE A.del_yn = 'N'");

            if (string.IsNullOrWhiteSpace(searchKeyword) == false)
            {
                if (keywordSearchType == 1) // 1: MemberSearchType.Name
                    sb.AppendLine(@"AND IFNULL(CONVERT(AES_DECRYPT(FROM_BASE64(B.name), 'dcc2b29aaa9f271d','\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8), '') LIKE CONCAT('%', @SearchKeyword, '%')");
                else if (keywordSearchType == 2) // 2: MemberSearchType.Email
                    sb.AppendLine(@"AND IFNULL(CONVERT(AES_DECRYPT(FROM_BASE64(A.email), 'dcc2b29aaa9f271d','\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8), '') LIKE CONCAT('%', @SearchKeyword, '%')");
                else if (keywordSearchType == 3) // 3: MemberSearchType.Phone
                    sb.AppendLine(@"AND IFNULL(CONVERT(AES_DECRYPT(FROM_BASE64(B.phone), '08a0d3a6ec32e85e','\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8), '') LIKE CONCAT('%', @SearchKeyword, '%')");
                else
                    throw new BizException(GlobalErrorCode.InvalidInputParameter.ToError());
            }

            if (string.IsNullOrWhiteSpace(fromDt) == false)
                sb.AppendLine("AND DATE_FORMAT(FROM_UNIXTIME(A.reg_dt), '%Y%m%d') BETWEEN @FromDt AND @ToDt");

            sb.AppendLine("ORDER BY A.reg_dt DESC, B.name ASC");
            sb.AppendLine("LIMIT @PageSize OFFSET @Offset;");

            sb.AppendLine("                    SELECT COUNT(1) AS TotalCount");
            sb.AppendLine("                      FROM tb_user A");
            sb.AppendLine("                     INNER JOIN tb_member B ");
            sb.AppendLine("                             ON A.uid = B.uid ");
            sb.AppendLine("                            AND B.userYn = 'Y'");
            sb.AppendLine("                     WHERE A.del_yn = 'N'");

            if (string.IsNullOrWhiteSpace(searchKeyword) == false)
            {
                if (keywordSearchType == 1) // 1: MemberSearchType.Name
                    sb.AppendLine(@"AND IFNULL(CONVERT(AES_DECRYPT(FROM_BASE64(B.name), 'dcc2b29aaa9f271d','\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8), '') LIKE CONCAT('%', @SearchKeyword, '%')");
                else if (keywordSearchType == 2) // 2: MemberSearchType.Email
                    sb.AppendLine(@"AND IFNULL(CONVERT(AES_DECRYPT(FROM_BASE64(A.email), 'dcc2b29aaa9f271d','\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8), '') LIKE CONCAT('%', @SearchKeyword, '%')");
                else if (keywordSearchType == 3) // 3: MemberSearchType.Phone
                    sb.AppendLine(@"AND IFNULL(CONVERT(AES_DECRYPT(FROM_BASE64(B.phone), '08a0d3a6ec32e85e','\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0') USING utf8), '') LIKE CONCAT('%', @SearchKeyword, '%')");
                else
                    throw new BizException(GlobalErrorCode.InvalidInputParameter.ToError());
            }

            if (string.IsNullOrWhiteSpace(fromDt) == false)
                sb.AppendLine("AND DATE_FORMAT(FROM_UNIXTIME(A.reg_dt), '%Y%m%d') BETWEEN @FromDt AND @ToDt");

            sb.AppendLine(";");

            sb.AppendLine("SET block_encryption_mode = 'aes-128-ecb';");
            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters);

            var users = (await multi.ReadAsync<SearchHospitalUsersResult>()).ToList();
            var totalCount = Convert.ToInt32(await multi.ReadSingleAsync<long>());

            var result = new ListResult<SearchHospitalUsersResult>
            {
                Items = users,
                TotalCount = totalCount
            };

            return result;
        }

        public async Task<GetHospitalUserProfileResult> GetHospitalUserProfileAsync(string userId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetHospitalUserProfileAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("UId", userId, DbType.String);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("  SELECT uid					AS Uid				");
                sb.AppendLine("     ,	mid					AS Mid				");
                sb.AppendLine("     ,	name				AS Name			    ");
                sb.AppendLine("     ,	pwd					AS Pwd				");
                sb.AppendLine("     ,	sns_id				AS SnsId			");
                sb.AppendLine("     ,	email				AS Email			");
                sb.AppendLine("     ,	birthday			AS Birthday		    ");
                sb.AppendLine("     ,	sex					AS Sex				");
                sb.AppendLine("     ,	phone				AS Phone			");
                sb.AppendLine("     ,	photo				AS Photo			");
                sb.AppendLine("     ,	del_yn				AS DelYn			");
                sb.AppendLine("     ,	login_type			AS LoginType		");
                sb.AppendLine("     ,	login_type_name		AS LoginTypeName	");
                sb.AppendLine("     ,	said				AS Said			    ");
                sb.AppendLine("     ,	reg_dt				AS RegDt			");
                sb.AppendLine("     ,	reg_dt_view			AS RegDtView		");
                sb.AppendLine("     ,	last_login_dt		AS LastLoginDt	    ");
                sb.AppendLine("     ,	last_login_dt_view_new	AS LastLoginDtView  ");
                sb.AppendLine("     ,	user_role	        AS UserRole         ");
                sb.AppendLine("    FROM VM_USERS VM                                ");
                sb.AppendLine("   WHERE uid = @UId and                              ");
                sb.AppendLine("     case when  last_login_dt_view_new is null then	");
                sb.AppendLine("     last_login_dt = (select max(last_login_dt) from VM_USERS                             	");
                sb.AppendLine("     where                             	");
                sb.AppendLine("     UID = @UId                             	");
                sb.AppendLine("     )         	");
                sb.AppendLine("     else 	");
                sb.AppendLine("     last_login_dt_view_new = (select max(last_login_dt_view_new) from VM_USERS                             	");
                sb.AppendLine("     where                             	");
                sb.AppendLine("     UID = @UId                             	");
                sb.AppendLine("     ) end	");
                #endregion

                using var connection = _connection.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<GetHospitalUserProfileResult>(sb.ToString(), parameters);

                if (result == null)
                    throw new BizException(AdminErrorCode.NotFoundUserInfo.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetHospitalUserProfileAsyncs() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<List<GetHospitalUserProfileResultFamilyItem>> GetHospitalUserFamilyProfileAsync(string userId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetHospitalUserProfileAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("UId", userId, DbType.String);

                var query = @"
                    SELECT uid AS UId,
                           mid AS MId,
                           name AS MemberNm,
                           birthday AS BirthDay,
                           sex AS Sex,
                           DATE_FORMAT(FROM_UNIXTIME(reg_dt),' %Y년 %m월 %d일') AS RegDt
                      FROM hello100.tb_member tm
                     WHERE uid = @UId 
                       AND userYn = 'N' 
                       AND del_yn='N'
                ";

                using var connection = _connection.CreateConnection();
                var result = (await connection.QueryAsync<GetHospitalUserProfileResultFamilyItem>(query, parameters)).ToList();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetHospitalUserProfileAsyncs() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<List<GetHospitalUserProfileResultServiceUsageItem>> GetHospitalUserAndFamilyServiceUsages(string userId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetHospitalUserProfileAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("UId", userId, DbType.String);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("  select	");
                sb.AppendLine("      te.req_date AS ReqDate,	");
                sb.AppendLine("      te.serial_no as SerialNo,	");
                sb.AppendLine("      CONVERT(AES_DECRYPT(FROM_BASE64(tm.name) , 'dcc2b29aaa9f271d') USING utf8) AS Name,	");
                sb.AppendLine("      tehi.hosp_no as HospNo,	");
                sb.AppendLine("       thi.name  as HospNm,	");
                sb.AppendLine("      (select max(doct_nm) from hello100_api.eghis_doct_info tedi where hosp_no = te.hosp_no and empl_no = te.empl_no) as DoctNm, 	");
                sb.AppendLine("      tc.cm_cd as ReceptType, 	");
                sb.AppendLine("      tc.cm_name as ReceptTypeNm,	");
                sb.AppendLine("      ifnull(erp.amount,0) as Amount,	");
                sb.AppendLine("      (select cm_name from hello100.tb_common tc where tc.cls_cd = '26' and tc.cm_cd = te.ptnt_state) as PtntStateNm,	");
                sb.AppendLine("      (select cm_name from hello100.tb_common tc where tc.cls_cd = '28' and tc.cm_cd = erp.process_status) as ProcessStatusNm	");
                sb.AppendLine("  from	");
                sb.AppendLine("      hello100.tb_eghis_hosp_receipt_info te 	");
                sb.AppendLine("      left outer join hello100_api.eghis_recept_payment erp on te.hosp_no =erp.hosp_no  and te.recept_no =erp.recept_no 	");
                sb.AppendLine("      left join hello100.tb_member tm on te.mid = tm.mid    	");
                sb.AppendLine("      left join hello100.tb_eghis_hosp_info tehi on te.hosp_no  = tehi.hosp_no	");
                sb.AppendLine("      left join hello100.tb_hospital_info thi on tehi.hosp_key  = thi.hosp_key 	");
                sb.AppendLine("      left join hello100.tb_common tc on te.recept_type = tc.cm_cd and tc.cls_cd=16     	");
                sb.AppendLine("  where	");
                sb.AppendLine("      te.uid=@UId	");
                sb.AppendLine("  order by 	");
                sb.AppendLine("      te.req_date desc	");

                using var connection = _connection.CreateConnection();
                var result = (await connection.QueryAsync<GetHospitalUserProfileResultServiceUsageItem>(sb.ToString(), parameters)).ToList();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetHospitalUserProfileAsyncs() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }
        #endregion
    }
}
