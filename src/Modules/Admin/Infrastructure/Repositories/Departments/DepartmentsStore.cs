using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Departments.Results;
using System.Data;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Departments
{
    public class DepartmentsStore : IDepartmentsStore
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<DepartmentsStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public DepartmentsStore(ILogger<DepartmentsStore> logger)
        {
            _logger = logger;
        }
        #endregion

        #region IDEPARTMENTSSTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<ListResult<GetDepartmentsResult>> GetDepartmentsAsync(DbSession db, string clsCd, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ClsCode", clsCd, DbType.String);

            var sql = @"
                SELECT cm_seq                             AS CmSeq,
                       cls_cd                             AS ClsCode,
                       cm_cd                              AS CmCode,
                       cls_name                           AS ClsName,
                       cm_name                            AS CmName,
                       locale                             AS Locale,
                       del_yn                             AS DelYn,
                       sort                               AS Sort,
                       DATE_FORMAT(reg_dt, '%Y-%m-%d %T') AS RegDate
                  FROM tb_common 
                 WHERE cls_cd = @ClsCode
                   AND del_yn = 'N'
                 ORDER BY sort asc; 

                SELECT COUNT(*) AS TotalCount 
                  FROM tb_common 
                 WHERE cls_cd = @ClsCode 
                   AND del_yn = 'N'
            ";

            var multi = await db.QueryMultipleAsync(sql, parameters, ct, _logger);

            var result = new ListResult<GetDepartmentsResult>();

            result.Items = (await multi.ReadAsync<GetDepartmentsResult>()).ToList();
            result.TotalCount = Convert.ToInt32(await multi.ReadSingleAsync<long>());

            return result;
        }

        #endregion
    }
}
