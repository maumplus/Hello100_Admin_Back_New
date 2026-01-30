using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using System.Data;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;
using Hello100Admin.Modules.Admin.Application.Common.Models;

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
            int pageNo, int pageSize, string? fromDt, string? toDt, int keywordSearchType, string? searchKeyword, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("SearchHospitalUsersAsync() Started");

                var fromDate = string.IsNullOrEmpty(fromDt) ? null : Convert.ToDateTime(fromDt).ToString("yyyyMMdd");
                var toDate = string.IsNullOrEmpty(toDt) ? null : Convert.ToDateTime(toDt).ToString("yyyyMMdd");

                var parameters = new DynamicParameters();
                parameters.Add("SearchKeyword", searchKeyword ?? string.Empty, DbType.String);
                parameters.Add("Limit", pageSize * pageNo, DbType.Int32);
                parameters.Add("OffSet", (pageNo - 1) * pageSize, DbType.Int32);
                parameters.Add("FromDt", fromDate, DbType.String);
                parameters.Add("ToDt", toDate, DbType.String);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
               
                if (keywordSearchType == 1) // 1: MemberSearchType.Name
                    sb.AppendLine($"CALL uSP_USER_SEARCH_GET_INFO_NEW(@SearchKeyword,'','','dcc2b29aaa9f271d','08a0d3a6ec32e85e',{(fromDate == null ? "null" : "@FromDt")}, {(toDate == null ? "null" : "@ToDt")}, @OffSet,@Limit);");
                else if (keywordSearchType == 2) // 2: MemberSearchType.Email
                    sb.AppendLine($"CALL uSP_USER_SEARCH_GET_INFO_NEW('',@SearchKeyword,'','dcc2b29aaa9f271d','08a0d3a6ec32e85e',{(fromDate == null ? "null" : "@FromDt")}, {(toDate == null ? "null" : "@ToDt")}, @OffSet,@Limit);");
                else if (keywordSearchType == 3) // 3: MemberSearchType.Phone
                    sb.AppendLine($"CALL uSP_USER_SEARCH_GET_INFO_NEW('','',@SearchKeyword,'dcc2b29aaa9f271d','08a0d3a6ec32e85e',{(fromDate == null ? "null" : "@FromDt")}, {(toDate == null ? "null" : "@ToDt")}, @OffSet,@Limit);");
                else
                    throw new BizException(GlobalErrorCode.InvalidInputParameter.ToError());
                #endregion

                using var connection = _connection.CreateConnection();
                var multi = await connection.QueryMultipleAsync(sb.ToString(), parameters);

                var queryList = (await multi.ReadAsync<SearchHospitalUsersResult>()).ToList();
                var totalCount = await multi.ReadSingleAsync<int>();

                var result = new ListResult<SearchHospitalUsersResult>
                {
                    Items = queryList,
                    TotalCount = totalCount
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SearchHospitalUsersAsync() Error");
                throw new BizException(GlobalErrorCode.DataQueryError.ToError());
            }
        }
        #endregion
    }
}
