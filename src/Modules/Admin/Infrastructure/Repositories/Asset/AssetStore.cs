using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Asset.Results;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Asset
{
    public class AssetStore : IAssetStore
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<IAssetStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public AssetStore(IDbConnectionFactory connection, ILogger<IAssetStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region IREQUESTSMANAGEMENTSTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<ListResult<GetUsageListResult>> GetUsageListAsync(
            int pageSize, int pageNum, int searchType, int searchDateType, string? fromDate, string? toDate, string? fromDay, string? toDay, string? searchKeyword
            , bool isExcel, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("GetUsageListAsync() Started");

                fromDate = Convert.ToDateTime(fromDate).ToString("yyyyMMdd");
                toDate = Convert.ToDateTime(toDate).ToString("yyyyMMdd");

                var parameters = new DynamicParameters();
                parameters.Add("PageSize", pageSize, DbType.Int32);
                parameters.Add("OffSet", (pageNum - 1) * pageSize, DbType.Int32);
                parameters.Add("FromDate", fromDate, DbType.String);
                parameters.Add("ToDate", toDate, DbType.String);
                parameters.Add("FromDay", fromDay, DbType.String);
                parameters.Add("ToDay", toDay, DbType.String);
                parameters.Add("SearchKeyword", searchKeyword, DbType.String);

                #region == Query ==
                var builder = new SqlBuilder();

                var listTemplate = builder.AddTemplate(@"
                    SELECT ROW_NUMBER() OVER (ORDER BY serial_key desc) AS RowNum
                           , serial_key			AS SerialKey		
                           , login_type			AS LoginType			
                           , main.hosp_no		AS HospNo			
                           , license_cd			AS LicenseCd		
                           , push_token			AS PushToken		
                           , empl_no			AS EmplNo			
                           , os_type			AS OsType		
                           , os_ver				AS OsVer			
                           , app_ver			AS AppVer			
                           , agent_ver			AS AgentVer			
                           , agent_ip			AS AgentIp			
                           , device_info		AS DeviceInfo		
                           , access_dt			AS AccessDt		
                           , before_access_dt	AS BeforeAccessDt	
                           , reg_dt				AS RegDt			
                           , DATEDIFF(curdate(), access_dt) AS  AccessDiffDay
                           , sub.name as HospName
                           , qr_check as QrCheck
                      FROM hello_desk.device_access_info main
                      left outer join
                      (select a.hosp_no, b.name from hello100.tb_eghis_hosp_info a
                         inner join hello100.tb_hospital_info b on a.hosp_key = b.hosp_key ) sub on main.hosp_no = sub.hosp_no
                     /**where**/
                     ORDER BY serial_key            
                ");

                var countTemplate = builder.AddTemplate(@"
                     SELECT COUNT(*) AS TotalCount
                      FROM hello_desk.device_access_info main
                      left outer join
                      (select a.hosp_no, b.name from hello100.tb_eghis_hosp_info a
                       inner join hello100.tb_hospital_info b on a.hosp_key = b.hosp_key ) sub on main.hosp_no = sub.hosp_no      
                     /**where**/
                     ;
                ");

                switch(searchDateType)
                {
                    case 1: //마지막 사용일
                        if (!string.IsNullOrWhiteSpace(fromDate)) builder.Where("DATE(access_dt) >= @FromDate");
                        if (!string.IsNullOrWhiteSpace(toDate)) builder.Where("DATE(access_dt) <= @ToDate");
                        break;
                    case 2: //최근 사용일
                        if (!string.IsNullOrWhiteSpace(fromDate)) builder.Where("DATE(before_access_dt) >= @FromDate");
                        if (!string.IsNullOrWhiteSpace(toDate)) builder.Where("DATE(before_access_dt) <= @ToDate");
                        break;

                }

                switch(searchType)
                {
                    case 1: //병원명
                        if (!string.IsNullOrWhiteSpace(searchKeyword)) builder.Where("sub.name like CONCAT('%', @SearchKeyword, '%')");
                        break;
                    case 2: //요양기관번호
                        if (!string.IsNullOrWhiteSpace(searchKeyword)) builder.Where("main.hosp_no like CONCAT('%', @SearchKeyword, '%')");
                        break;
                    case 3: //미사용 경과일
                        if (!string.IsNullOrWhiteSpace(fromDay)) builder.Where("DATEDIFF(curdate(), access_dt) >= @FromDay");
                        if (!string.IsNullOrWhiteSpace(toDay)) builder.Where("DATEDIFF(curdate(), access_dt) <= @ToDay");
                        break;
                }

                // 엑셀의 경우 LIMIT 구문을 제거하기 위해 isExcel param을 활용
                // 같은 SQL을 사용하는 여러개의 Store를 만들고 싶지 않아서
                string listSql = listTemplate.RawSql;

                listSql += isExcel ? ";" : " LIMIT @OffSet, @PageSize;";

                var sql = listSql + countTemplate.RawSql;
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

                var queryList = (await multi.ReadAsync<GetUsageListResult>()).AsList();
                var totalCount = await multi.ReadSingleAsync<int>();

                var result = new ListResult<GetUsageListResult>
                {
                    Items = queryList,
                    TotalCount = totalCount
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetUsageListAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }
        #endregion
    }
}
