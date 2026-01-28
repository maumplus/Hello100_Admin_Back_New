using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Bank;
using Hello100Admin.Modules.Seller.Application.Features.Bank.ReadModels.GetBankList;
using Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Bank;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Hello100Admin.Modules.Seller.Infrastructure.Repositories.Bank
{
    public class BankStore : IBankStore
    {
        #region FIELD AREA **********************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<BankStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA ************************************************
        public BankStore(IDbConnectionFactory connection, ILogger<BankStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region ISELLERQUERIES IMPLEMENTS AREA ************************************************
        public async Task<List<GetBankListReadModel>> GetBankListAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetBankListAsync()");

                var parameters = new DynamicParameters();
                parameters.Add("@Enable", "1", DbType.Int32);

                string query = @"
                    SELECT id AS Id,
                           type AS Type,
                           name AS Name,
                           code AS Code,
                           img_path AS ImgPath
                      FROM tb_bank_code
                     WHERE enable = @Enable
                     ORDER BY Type ASC, Name ASC
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = (await connection.QueryAsync<GetBankListRow>(query, parameters)).ToList();

                var result = queryResult.Adapt<List<GetBankListReadModel>>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetBankListAsync() 은행 리스트 조회 실패");
                return new List<GetBankListReadModel>();
            }
        }
        #endregion
    }
}
