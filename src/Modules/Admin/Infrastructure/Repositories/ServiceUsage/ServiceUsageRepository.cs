using Dapper;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.ServiceUsage
{
    public class ServiceUsageRepository : IServiceUsageRepository
    {
        #region FIELD AREA ****************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<ServiceUsageRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public ServiceUsageRepository(IDbConnectionFactory connection, ILogger<ServiceUsageRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region GENERAL METHOD AREA **************************************
        public async Task<int> SubmitAlimtalkApplicationAsync(SubmitAlimtalkApplicationCommand req, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("SubmitAlimtalkApplicationAsync HospNo: [{HospNo}]", req.HospNo);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("HospNo", req.HospNo, DbType.String);
                parameters.Add("HospKey", req.HospKey, DbType.String);
                parameters.Add("DoctNm", req.DoctNm, DbType.String);
                parameters.Add("DoctTel", req.DoctTel, DbType.String);
                parameters.Add("TmpType", req.TmpType, DbType.String);

                var sql = @"
                    INSERT INTO hello100.tb_kakao_msg_join
                                ( hosp_no, hosp_key, doct_nm, doct_tel, reg_dt, tmp_type )
                         VALUES
                                ( @HospNo, @HospKey, @DoctNm, @DoctTel, UNIX_TIMESTAMP(NOW()), @TmpType )
                ";

                using var connection = _connection.CreateConnection();
                var result = await connection.ExecuteAsync(sql, parameters);

                if (result != 1)
                    throw new BizException(GlobalErrorCode.DataInsertError.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("SubmitAlimtalkApplicationAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }
        #endregion
    }
}
