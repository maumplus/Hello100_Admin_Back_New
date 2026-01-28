using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ApprovalRequest;
using Microsoft.Extensions.Logging;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Mapster;
using System.Text;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.ApprovalRequest;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.ReadModels;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.ApprovalRequest
{
    public class ApprovalRequestStore : IApprovalRequestStore
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<ApprovalRequestStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public ApprovalRequestStore(IDbConnectionFactory connection, ILogger<ApprovalRequestStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region IAPPROVALREQUESTSTORE IMPLEMENTS METHOD AREA ***************************************
        public async Task<GetUntactMedicalRequestsForApprovalReadModel> GetUntactMedicalRequestsForApprovalAsync(int pageNo, int pageSize, string hospKey, string apprYn, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetUntactMedicalRequestsForApprovalAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("HospKey", hospKey, DbType.String);
                parameters.Add("Limit", pageSize, DbType.Int32);
                parameters.Add("OffSet", (pageNo - 1) * pageSize, DbType.Int32);

                #region == Query ==
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(" SET @total_count:= (SELECT COUNT(*)                                     ");
                sb.AppendLine("                       FROM tb_eghis_doct_untact_join                             ");
                sb.AppendLine("                      WHERE hosp_key = @HospKey    ");
                if (apprYn == "N")
                {
                    sb.AppendLine("  and  join_state='01' 	");
                }
                sb.AppendLine("                     ); ");
                sb.AppendLine(" SET @row_num:=@total_count + 1 - @OffSet;         ");

                sb.AppendLine("  select 	@row_num:=@row_num - 1 AS RowNum,  	");
                sb.AppendLine("      hosp_key as HospKey,	");
                sb.AppendLine("      seq as Seq,	");
                sb.AppendLine("      (select cm_name from hello100.tb_common tc where cls_cd='25' and tc.cm_cd=x.join_state) as JoinStateNm,	");
                sb.AppendLine("      join_state as JoinState,	");
                sb.AppendLine("      from_unixtime(reg_dt, '%Y-%m-%d %H:%i')  as RegDt,	");
                sb.AppendLine("      from_unixtime(x.join_state_mod_dt, '%Y-%m-%d %H:%i')  as ModDt,	");
                sb.AppendLine("       case when join_state_mod_dt !=0 then  from_unixtime(join_state_mod_dt, '%Y-%m-%d %H:%i') else '' end as JoinStateModDt,	");
                sb.AppendLine("       doct_nm as DoctNm,	");
                sb.AppendLine("       doct_tel as DoctTel,	");
                sb.AppendLine("       return_reason as ReturnReason	");
                sb.AppendLine("  from	");
                sb.AppendLine("      tb_eghis_doct_untact_join x	");
                sb.AppendLine("  where	");
                sb.AppendLine("      x.hosp_key =@HospKey	");
                if (apprYn == "N")
                {
                    sb.AppendLine("     and join_state='01' 	");
                }
                sb.AppendLine("  order by 	");
                sb.AppendLine("      x.reg_dt desc	");
                sb.AppendLine("   LIMIT @OffSet, @Limit;");

                sb.AppendLine("  SELECT @total_count;   ");
                #endregion

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(sb.ToString(), parameters);

                var result = new GetUntactMedicalRequestsForApprovalReadModel();

                var tempList = (await multi.ReadAsync<GetUntactMedicalRequestsForApprovalRow>()).ToList();
                var tempTotalCount = await multi.ReadSingleAsync<long>();

                result.List = tempList.Adapt<List<GetUntactMedicalRequestsForApprovalItemReadModel>>();
                result.TotalCount = Convert.ToInt32(tempTotalCount);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetUntactMedicalRequestsForApprovalAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }

        public async Task<GetUntactMedicalRequestDetailForApprovalReadModel> GetUntactMedicalRequestDetailForApprovalAsync(int seq, string imageUrl, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetUntactMedicalRequestsForApprovalAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("Seq", seq, DbType.Int32);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("   select 	");
                sb.AppendLine("       seq as Seq,    	");
                sb.AppendLine("       x.hosp_no as HospNo,   	");
                sb.AppendLine("       x.hosp_key as HospKey, 	");
                sb.AppendLine("       y.name as HospNm,  	");
                sb.AppendLine("       y.addr as Addr,    	");
                sb.AppendLine("       x.empl_no as EmplNo,   	");
                sb.AppendLine("       x.hosp_tel as HospTel, 	");
                sb.AppendLine("       x.post_cd as PostCd,   	");
                sb.AppendLine("       doct_no as DoctNo, 	");
                sb.AppendLine("       doct_no_type as DoctNoType,    	");
                sb.AppendLine("       (select cm_name from tb_common tc where tc.cls_cd = '23' and tc.cm_cd = x.doct_no_type) as DoctNoTypeNm,    	");
                sb.AppendLine("       doct_license_file_seq as DoctLicenseFileSeq,   	");
                sb.AppendLine("       doct_nm as DoctNm, 	");
                sb.AppendLine("       doct_birthday as DoctBirthday, 	");
                sb.AppendLine("       doct_tel as DoctTel,   	");
                sb.AppendLine("       doct_intro as DoctIntro,   	");
                sb.AppendLine("       doct_file_seq as DoctFileSeq,  	");
                sb.AppendLine("       doct_history as DoctHistory,   	");
                sb.AppendLine("       clinic_time as ClinicTime, 	");
                sb.AppendLine("       clinic_guide as ClinicGuide,   	");
                sb.AppendLine("       account_info_file_seq as AccountInfoFileSeq,   	");
                sb.AppendLine("       join_state as JoinState,   	");
                sb.AppendLine("       from_unixtime(x.reg_dt, '%Y-%m-%d %H:%i') as RegDt ,	");
                sb.AppendLine("       concat('" + imageUrl + "',(select file_path from tb_file_info tfi where tfi.seq= x.doct_file_seq )) as DoctFilePath,	");
                sb.AppendLine("       concat('" + imageUrl + "',(select file_path from tb_file_info tfi where tfi.seq= x.business_file_seq )) as BusinessFilePath,	");
                sb.AppendLine("       concat('" + imageUrl + "',(select file_path from tb_file_info tfi where tfi.seq= x.doct_license_file_seq )) as LicenseFilePath,	");
                sb.AppendLine("       concat('" + imageUrl + "',(select file_path from tb_file_info tfi where tfi.seq= x.account_info_file_seq )) as AccountFilePath,	");
                sb.AppendLine("       concat('" + imageUrl + "',(select cm_name from tb_common tc where tc.cls_cd='25' and tc.cm_cd=x.join_state)) as JoinStateNm,	");
                sb.AppendLine("       x.return_reason as ReturnReason  ");
                sb.AppendLine("  from    	");
                sb.AppendLine("      tb_eghis_doct_untact_join x left join tb_hospital_info y on x.hosp_key  = y.hosp_key    	");
                sb.AppendLine("  where 	");
                sb.AppendLine("      seq = @Seq ");
                #endregion

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<GetUntactMedicalRequestDetailForApprovalRow>(sb.ToString(), parameters);

                var result = queryResult.Adapt<GetUntactMedicalRequestDetailForApprovalReadModel>();

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetUntactMedicalRequestsForApprovalAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }
        #endregion
    }
}
