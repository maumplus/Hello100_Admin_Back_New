using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.CreateSeller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerDetail;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerRemitWaitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.UpdateSellerRemit;
using Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Hello100Admin.Modules.Seller.Infrastructure.Repositories.Seller
{
    public class SellerStore : ISellerStore
    {
        #region FIELD AREA **********************************************
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<SellerStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA ************************************************
        public SellerStore(IDbConnectionFactory connection, ILogger<SellerStore> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        #endregion

        #region ISELLERQUERIES IMPLEMENTS AREA ************************************************
        public async Task<GetApprovedUntactHospitalInfoReadModel?> GetApprovedUntactHospitalInfoAsync(string hospNo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetApprovedUntactHospitalInfoAsync() by HospNo: {HospNo}", hospNo);

                var parameters = new DynamicParameters();
                parameters.Add("@HospNo", hospNo, DbType.String);

                string query = @"
                    SELECT tehi.hosp_key AS HospKey,
                           tehi.hosp_no AS HospNo,
                           thi.name AS HospName,
                           tehi.business_no AS BusinessNo,
                           tehi.chart_type AS ChartType,
                           tehi.business_level AS BusinessLevel,
                           thi.addr AS HospAddr,
                           thi.post_cd AS HospPostCd,
                           thi.tel AS HospTel
                      FROM tb_eghis_hosp_info tehi
                     INNER JOIN tb_hospital_info thi 
                             ON thi.hosp_key = tehi.hosp_key
                     WHERE EXISTS ( SELECT null
                                      FROM tb_eghis_doct_untact_join teduj
                                     WHERE teduj.hosp_key = tehi.hosp_key
                                       AND teduj.join_state = '02' )
                       AND tehi.hosp_no = @HospNo;
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<GetApprovedUntactHospitalInfoRow>(query, parameters);

                var result = queryResult?.Adapt<GetApprovedUntactHospitalInfoReadModel>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetApprovedUntactHospitalInfoAsync() by HospNo: {HospNo}", hospNo);
                throw;
            }
        }

        public async Task<long> GetHospitalSellerCountAsync(string hospNo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetHospitalSellerCountAsync() by HospNo: {HospNo}", hospNo);

                var parameters = new DynamicParameters();
                parameters.Add("@HospNo", hospNo, DbType.String);

                string query = @"
                    SELECT COUNT(*)
                      FROM tb_hosp_seller
                     WHERE hosp_no = @HospNo
                ";

                using var connection = _connection.CreateConnection();
                var result = await connection.ExecuteScalarAsync<long>(query, parameters);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetHospitalSellerCountAsync() by HospNo: {HospNo}", hospNo);
                return -1;
            }
        }

        public async Task<List<GetHospSellerListReadModel>> GetHospSellerListAsync(GetSellerListQuery req, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetHospSellerListAsync() Started");

                int offset = (req.PageNo - 1) * req.PageSize;

                var parameters = new DynamicParameters();
                parameters.Add("SearchText", req.SearchText, DbType.String);
                parameters.Add("PageSize", req.PageSize, DbType.Int32);
                parameters.Add("Offset", offset, DbType.Int32);
                parameters.Add("Enabled", req.Enabled ? "1" : "0", DbType.String);

                string whereSql = string.Empty;

                if (string.IsNullOrEmpty(req.IsSync) == false)
                {
                    whereSql += " AND ths.is_sync =  @IsSync";
                    parameters.Add("IsSync", req.IsSync, DbType.String);
                }

                string query = $@"
                    SELECT *
                      FROM ( SELECT COUNT(*) OVER()                            AS TotalCount,
                                    ROW_NUMBER() OVER(ORDER BY ths.reg_dt ASC) AS RowNum,
                                    ths.id                                     AS Id,
                                    thi.name                                   AS HospName,
                                    tehi.business_no                           AS BusinessNo,
                                    tehi.business_level                        AS BusinessLevel,
                                    tehi.hosp_no                               AS HospNo,
                                    tehi.chart_type                            AS ChartType,
                                    ths.bank_cd                                AS BankCd,
                                    tbc.name                                   AS BankName,
                                    tbc.img_path                               AS BankImgPath,
                                    ths.deposit_no                             AS DepositNo,
                                    ths.depositor                              AS Depositor,
                                    ths.enabled                                AS Enabled,
                                    ths.etc                                    AS Etc,
                                    ths.is_sync                                AS IsSync,
                                    ths.reg_aid                                AS RegAid,
                                    ths.reg_dt                                 AS RegDt,
                                    ths.mod_dt                                 AS ModDt
                               FROM tb_hosp_seller ths
                              INNER JOIN tb_eghis_hosp_info tehi 
                                      ON tehi.hosp_no = ths.hosp_no
                              INNER JOIN tb_hospital_info thi 
                                      ON thi.hosp_key = tehi.hosp_key
                               LEFT JOIN tb_bank_code tbc 
                                      ON tbc.code = ths.bank_cd
                              WHERE 1 = 1
                                AND ( thi.name LIKE CONCAT('%', @SearchText, '%')
                                   OR tehi.hosp_no LIKE CONCAT('%', @SearchText, '%') )
                                AND ths.enabled = @Enabled
                              {whereSql}
                           ) AS sub
                     ORDER BY sub.regDt DESC
                     LIMIT @PageSize OFFSET @Offset;
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = (await connection.QueryAsync<GetHospSellerListRow>(query, parameters)).ToList();

                var result = queryResult.Adapt<List<GetHospSellerListReadModel>>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetHospSellerListAsync()"); // 셀러 리스트 조회 시 오류
                return new List<GetHospSellerListReadModel>();
            }
        }

        public async Task<GetHospSellerDetailInfoReadModel?> GetHospSellerDetailInfoAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetHospSellerDetailInfoAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);

                string query = @"
                    SELECT ths.id                   AS Id,
                           thi.name                 AS HospName,
                           tehi.business_no         AS BusinessNo,
                           tehi.business_level      AS BusinessLevel,
                           tehi.hosp_no             AS HospNo,
                           tehi.chart_type          AS ChartType,
                           ths.bank_cd              AS BankCd,
                           tbc.name                 AS BankName,
                           tbc.img_path             AS BankImgPath,
                           ths.deposit_no           AS DepositNo,
                           ths.depositor            AS Depositor,
                           ths.enabled              AS Enabled,
                           ths.etc                  AS Etc,
                           ths.is_sync              AS IsSync,
                           ths.reg_dt               AS RegDt,
                           ths.mod_dt               AS ModDt
                      FROM tb_hosp_seller ths
                     INNER JOIN tb_eghis_hosp_info tehi 
                             ON tehi.hosp_no = ths.hosp_no
                     INNER JOIN tb_hospital_info thi 
                             ON thi.hosp_key = tehi.hosp_key
                      LEFT JOIN tb_bank_code tbc 
                             ON tbc.code = ths.bank_cd
                     WHERE ths.id = @Id;
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<GetHospSellerDetailInfoRow>(query, parameters);

                var result = queryResult?.Adapt<GetHospSellerDetailInfoReadModel>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetHospSellerDetailInfoAsync()");
                return null;
            }
        }

        public async Task<GetSellerRemitCountReadModel?> GetSellerRemitCountAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetSellerRemitCountAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);

                string query = @"
                    SELECT SUM(CASE WHEN status = '0' THEN amount ELSE 0 END) AS PendingAmount,
                           SUM(CASE WHEN status = '1' THEN amount ELSE 0 END) AS RequestAmount,
                           SUM(CASE WHEN status = '2' THEN amount ELSE 0 END) AS SuccessAmount,
                           SUM(CASE WHEN status = '4' THEN amount ELSE 0 END) AS FailAmount,
                           SUM(CASE WHEN status = '5' THEN amount ELSE 0 END) AS DeleteAmount,
                           SUM(CASE WHEN status = '0' THEN 1 ELSE 0 END) AS PendingCount,
                           SUM(CASE WHEN status = '1' THEN 1 ELSE 0 END) AS RequestCount,
                           SUM(CASE WHEN status = '2' THEN 1 ELSE 0 END) AS SuccessCount,
                           SUM(CASE WHEN status = '4' THEN 1 ELSE 0 END) AS FailCount,
                           SUM(CASE WHEN status = '5' THEN 1 ELSE 0 END) AS DeleteCount
                      FROM tb_seller_remit
                     WHERE hosp_seller_id = @Id;
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<GetSellerRemitCountRow>(query, parameters);

                var result = queryResult?.Adapt<GetSellerRemitCountReadModel>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetSellerRemitCountAsync()");
                return null;
            }
        }

        public async Task<GetHospSellerRemitWaitInfoReadModel?> GetHospSellerRemitWaitInfoAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetHospSellerRemitWaitInfoAsync() Started");

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);

                string query = @"
                    SELECT tsr.id              AS Id,
                           ths.seller_id       AS SellerId,
                           ths.deposit_no      AS DepositNo,
                           ths.depositor       AS Depositor,
                           ths.enabled         AS Enabled,
                           ths.is_sync         AS IsSync,
                           tsr.amount          AS Amount,
                           tsr.va_name         AS VaName,
                           tsr.status          AS Status
                      FROM tb_seller_remit tsr
                     INNER JOIN tb_hosp_seller ths 
                             ON ths.id = tsr.hosp_seller_id
                     WHERE tsr.id = @Id
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = await connection.QueryFirstOrDefaultAsync<GetHospSellerRemitWaitInfoRow>(query, parameters);

                var result = queryResult?.Adapt<GetHospSellerRemitWaitInfoReadModel>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetHospSellerRemitWaitInfoAsync()");
                return null;
            }
        }

        public async Task<List<GetHospSellerRemitListReadModel>> GetHospSellerRemitListAsync(GetSellerRemitListQuery req, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetHospSellerRemitListAsync() Started");

                int offset = (req.PageNo - 1) * req.PageSize;

                int startUnix = (int)DateTimeOffset.Parse(req.StartDt).ToUnixTimeSeconds();
                int endUnix = (int)DateTimeOffset.Parse(req.EndDt).AddDays(1).AddSeconds(-1).ToUnixTimeSeconds();

                var parameters = new DynamicParameters();
                parameters.Add("@StartDt", startUnix, DbType.Int32);
                parameters.Add("@EndDt", endUnix, DbType.Int32);
                parameters.Add("@PageSize", req.PageSize, DbType.Int32);
                parameters.Add("@Offset", offset, DbType.Int32);

                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(req.SearchText))
                {
                    parameters.Add("@SearchText", req.SearchText, DbType.String);

                    whereSql += @"
                        AND (
                            thi.name LIKE CONCAT('%', @SearchText, '%')
                            OR tehi.hosp_no LIKE CONCAT('%', @SearchText, '%')
                        )";
                }

                if (!string.IsNullOrEmpty(req.RemitStatus))
                {
                    parameters.Add("@Status", req.RemitStatus, DbType.String);

                    whereSql += " AND tsr.status = @Status";
                }

                string query = $@"
                    SELECT *
                      FROM ( SELECT COUNT(*) OVER()                            AS TotalCount,
                                    ROW_NUMBER() OVER(ORDER BY tsr.res_dt ASC) AS RowNum,
                                    tsr.id                                     AS Id,
                                    thi.name                                   AS HospName,
                                    tehi.hosp_no                               AS HospNo,
                                    tsr.hosp_seller_id                         AS HospSellerId,
                                    tsr.amount                                 AS Amount,
                                    tsr.va_mny                                 AS VaMny,
                                    tsr.va_name                                AS VaName,
                                    tsr.cust_ip                                AS CustIp,
                                    tsr.status                                 AS Status,
                                    tsr.requested = '1'                        AS Requested,
                                    tsr.trade_seq                              AS TradeSeq,
                                    tsr.trade_date                             AS TradeDate,
                                    tsr.bal_amount                             AS BalAmount,
                                    tsr.bank_cd                                AS BankCd,
                                    tsr.app_time                               AS AppTime,
                                    tsr.van_apptime                            AS VanApptime,
                                    tsr.bank_name                              AS BankName,
                                    tbc.img_path                               AS BankImgPath,
                                    tsr.account                                AS Account,
                                    tsr.depositor                              AS Depositor,
                                    tsr.res_cd                                 AS ResCd,
                                    tsr.res_msg                                AS ResMsg,
                                    tsr.res_en_msg                             AS ResEnMsg,
                                    tsr.reg_aid                                AS RegAid,
                                    tsr.reg_dt                                 AS RegDt,
                                    tsr.res_dt                                 AS ResDt,
                                    tsr.refresh_dt                             AS RefreshDt,
                                    tsr.etc                                    AS Etc
                               FROM tb_seller_remit tsr
                              INNER JOIN tb_hosp_seller ths 
                                      ON ths.id = tsr.hosp_seller_id
                              INNER JOIN tb_eghis_hosp_info tehi 
                                      ON tehi.hosp_no = ths.hosp_no
                              INNER JOIN tb_hospital_info thi 
                                      ON thi.hosp_key = tehi.hosp_key
                               LEFT JOIN tb_bank_code tbc 
                                      ON tbc.code = ths.bank_cd
                              WHERE 1 = 1
                                AND tsr.res_dt BETWEEN @StartDt AND @EndDt
                              {whereSql} ) AS sub
                     ORDER BY sub.ResDt DESC
                     LIMIT @PageSize OFFSET @Offset;
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = (await connection.QueryAsync<GetHospSellerRemitListRow>(query, parameters)).ToList();

                var result = queryResult.Adapt<List<GetHospSellerRemitListReadModel>>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetHospSellerRemitListAsync()");
                return new List<GetHospSellerRemitListReadModel>();
            }
        }

        public async Task<List<GetSellerRemitWaitListReadModel>> GetSellerRemitWaitListAsync(string startDt, string endDt, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetSellerRemitWaitListAsync() Started");

                int startUnix = (int)DateTimeOffset.Parse(startDt).ToUnixTimeSeconds();
                int endUnix = (int)DateTimeOffset.Parse(endDt).AddDays(1).AddSeconds(-1).ToUnixTimeSeconds();

                var parameters = new DynamicParameters();
                parameters.Add("@StartDt", startUnix, DbType.Int32);
                parameters.Add("@EndDt", endUnix, DbType.Int32);

                string query = @"
                    SELECT ROW_NUMBER() OVER(ORDER BY tsr.id ASC) AS RowNum,
                           tsr.id                                 AS Id,
                           thi.name                               AS HospName,
                           tehi.hosp_no                           AS HospNo,
                           ths.bank_cd                            AS BankCd,
                           tbc.name                               AS BankName,
                           tbc.img_path                           AS BankImgPath,
                           ths.deposit_no                         AS DepositNo,
                           ths.depositor                          AS Depositor,
                           tsr.amount                             AS Amount,
                           tsr.status                             AS Status,
                           tsr.etc                                AS Etc
                      FROM tb_seller_remit tsr
                     INNER JOIN tb_hosp_seller ths 
                             ON ths.id = tsr.hosp_seller_id
                     INNER JOIN tb_eghis_hosp_info tehi 
                             ON tehi.hosp_no = ths.hosp_no
                     INNER JOIN tb_hospital_info thi 
                             ON thi.hosp_key = tehi.hosp_key
                      LEFT JOIN tb_bank_code tbc 
                             ON tbc.code = ths.bank_cd
                     WHERE tsr.status = '0'
                       AND tsr.reg_dt BETWEEN @StartDt AND @EndDt
                     ORDER BY tsr.id DESC
                ";

                using var connection = _connection.CreateConnection();
                var queryResult = (await connection.QueryAsync<GetSellerRemitWaitListRow>(query, parameters)).ToList();

                var result = queryResult.Adapt<List<GetSellerRemitWaitListReadModel>>();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetSellerRemitWaitListAsync() 송금 요청(대기) 리스트 조회 시 오류");
                return new List<GetSellerRemitWaitListReadModel>();
            }
        }
        #endregion
    }
}
