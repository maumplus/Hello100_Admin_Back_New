using Dapper;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Seller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSeller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSellerRemit;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemit;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;

namespace Hello100Admin.Modules.Seller.Infrastructure.Repositories.Seller
{
    public class SellerRepository : ISellerRepository
    {
        #region FIELD AREA *****************************************
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<SellerRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA ***********************************
        public SellerRepository(IDbConnectionFactory connectionFactory, ILogger<SellerRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        #endregion

        #region GENERAL METHOD AREA ******************************
        public async Task<long> InsertTbHospSellerAsync(CreateSellerCommand req, string sellerId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("InsertTbHospSellerAsync: [{sellerId}], [{aid}]", sellerId, req.Aid);

                if (string.IsNullOrEmpty(req.Aid) ||
                    string.IsNullOrEmpty(req.HospNo) ||
                    string.IsNullOrEmpty(req.BankCd) ||
                    string.IsNullOrEmpty(req.DepositNo) ||
                    string.IsNullOrEmpty(req.Depositor))
                {
                    return -1;
                }

                var adminCount = await GetTbAdminCountByAIdAsync(req.Aid, cancellationToken);
                if (adminCount <= 0)
                {
                    _logger.LogError("InsertTbHospSellerAsync: 관리자 정보 없음");
                    return -2; // 관리자 정보 없음 AdminNotFound
                }

                var eghisHospKey = await GetEghisHospKeyByHospNoAsync(req.HospNo);
                if (string.IsNullOrEmpty(eghisHospKey) == true)
                {
                    _logger.LogError("InsertTbHospSellerAsync: 병원 정보 없음");
                    return -3; // 병원 연동 정보 없음 EghisHospNotFound
                }

                var hospCount = await GetTbHospitalCountByHospKeyAsync(eghisHospKey);
                if (hospCount <= 0)
                {
                    _logger.LogError("InsertTbHospSellerAsync: 심평원 병원 정보 없음");
                    return -4; // 심평원 병원 정보 없음 HospNotFound
                }

                var entity = new InsertHospSellerParams
                {
                    HospNo = req.HospNo,
                    SellerId = sellerId,
                    BankCd = req.BankCd,
                    DepositNo = req.DepositNo,
                    Depositor = req.Depositor,
                    Etc = req.Etc,
                    RegAId = req.Aid,
                    RegDt = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                long result = await InsertTbHospSellerAsync(entity, cancellationToken);

                if (result > 0)
                {
                    return result; // Success
                }
                else
                {
                    _logger.LogError("InsertTbHospSellerAsync: 데이터 저장 실패, Param: {Param}", req.ToJsonForStorage());
                    return -1; // NotInsertData
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertTbHospSellerAsync: 관리자 셀러 등록 실패 {Exception}", ex);
                return -500; // Exception
            }
        }

        public async Task<int> UpdateTbHospSellerIsSyncByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UpdateTbHospSellerIsSyncByIdAsync by Id: [{Id}]", id);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);

                var sql = @"
                    UPDATE tb_hosp_seller
                       SET is_sync = '1'
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(sql, parameters);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateTbHospSellerIsSyncByIdAsync: 관리자 셀러 업데이트(KCP 싱크) 실패 {Exception}", ex);
                return -1;
            }
        }

        public async Task<long> InsertTbHospSellerRemitAsync(CreateSellerRemitCommand req, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("InsertTbHospSellerRemitAsync by AId: [{Id}]", req.Aid);

                var depositor = await GetHospSellerDepositorAsync(req.HospSellerId);

                if (string.IsNullOrEmpty(depositor) == true)
                {
                    return -1;
                }

                var entity = new InsertHospSellerRemitParams
                {
                    HospSellerId = req.HospSellerId,
                    Amount = req.Amount,
                    VaMny = req.Amount,
                    VaName = "(주)이지스헬스케어",
                    Depositor = depositor,
                    Etc = req.Etc,
                    RegAId = req.Aid,
                    RegDt = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                long result = await this.InsertTbHospSellerRemitAsync(entity, cancellationToken);

                if (result <= 0)
                {
                    return -2;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("InsertTbHospSellerRemitAsync: 송금 요청 임시 등록 실패 {Exception}", ex);
                return -3;
            }
        }

        public async Task<int> UpdateSellerRemitAsync(UpdateSellerRemitParams param, int id, string? etc, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UpdateSellerRemitAsync by Id: [{Id}]", id);

                var remitInfo = await this.GetSellerRemitInfoByIdAsync(id, cancellationToken);

                if (remitInfo == null) return -1;

                var result = await this.UpdateSellerRemitAsync(param, remitInfo, id, etc, cancellationToken);

                if (result <= 0) return -2;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateSellerRemitAsync: 송금 결과 업데이트 실패 {Exception}", ex);
                return -3;
            }
        }

        public async Task<int> DeleteSellerRemitAsync(int id, string? etc, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("DeleteSellerRemitAsync by Id: [{Id}]", id);

                var status = await this.GetSellerRemitStatusByIdAsync(id, cancellationToken);

                if (string.IsNullOrEmpty(status) == true) return -1; // 데이터가 없습니다.

                if (status == "2")
                {
                    // 송금 완료 건입니다. 삭제가 불가능 합니다.
                    return -2;
                }

                if (status == "5")
                {
                    // 이미 삭제가 된 건입니다.
                    return -3;
                }

                var result = await this.UpdateSellerRemitAsync(id, etc, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteSellerRemitAsync: 송금 결과 삭제 실패 {Exception}", ex);
                return -5;
            }
        }

        public async Task<int> UpdateSellerRemitEnabledAsync(int id, bool enabled, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UpdateSellerRemitEnabledAsync by Id: [{Id}]", id);

                var sellerCount = await this.GetHospSellerCountAsync(id, cancellationToken);

                if (sellerCount <= 0)
                {
                    _logger.LogError("UpdateSellerRemitEnabledAsync: 관리자 셀러 정보 없음");
                    return -1;
                }

                var result = await this.UpdateSellerRemitEnabledAsync(id, enabled ? "1" : "0", cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateSellerRemitEnabledAsync: 관리자 셀러 업데이트(KCP 싱크) 실패 {Exception}", ex);
                return -1;
            }
        }
        #endregion

        #region INTERNAL METHOD AREA ********************************************
        private async Task<double> GetTbAdminCountByAIdAsync(string aId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetTbAdminCountByAIdAsync by AId: [{AId}]", aId);

                var sql = @"
                    SELECT COUNT(*) 
                      FROM tb_admin 
                     WHERE aid = @AId 
                       AND del_yn = 'N';
                ";

                using var connection = _connectionFactory.CreateConnection();
                var adminCount = await connection.ExecuteScalarAsync<double>(sql, new { AId = aId });

                return adminCount;
            }
            catch (Exception e)
            {
                _logger.LogError("GetTbAdminCountByAIdAsync: 관리자 계정 조회 실패 {Exception}", e);
                return -1;
            }
        }

        private async Task<string?> GetEghisHospKeyByHospNoAsync(string hospNo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetEghisHospKeyByHospNoAsync by HospNo: [{HospNo}]", hospNo);

                var sql = @"
                    SELECT hosp_key
                      FROM tb_eghis_hosp_info 
                     WHERE hosp_no = @HospNo;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<string>(sql, new { HospNo = hospNo });

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("GetEghisHospKeyByHospNoAsync: 병원 키 값 조회 실패 {Exception}", e);
                return null;
            }
        }

        private async Task<long> GetTbHospitalCountByHospKeyAsync(string hospKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetTbHospitalCountByHospKeyAsync by HospKey: [{HospKey}]", hospKey);

                var sql = @"
                    SELECT COUNT(*)
                      FROM tb_hospital_info 
                     WHERE hosp_key = @HospKey;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<long>(sql, new { HospKey = hospKey });

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("GetTbHospitalCountByHospKeyAsync: 병원 키 값 조회 실패 {Exception}", e);
                return -1;
            }
        }

        private async Task<long> InsertTbHospSellerAsync(InsertHospSellerParams hospSeller, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("InsertTbHospSellerAsync SellerId: [{SellerId}]", hospSeller.SellerId);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("HospNo", hospSeller.HospNo, DbType.String);
                parameters.Add("SellerId", hospSeller.SellerId, DbType.String);
                parameters.Add("BankCd", hospSeller.BankCd, DbType.String);
                parameters.Add("DepositNo", hospSeller.DepositNo, DbType.String);
                parameters.Add("Depositor", hospSeller.Depositor, DbType.String);
                parameters.Add("Etc", hospSeller.Etc, DbType.String);
                parameters.Add("RegAId", hospSeller.RegAId, DbType.String);
                parameters.Add("RegDt", (double)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DbType.Int64);

                var sql = @"
                    INSERT INTO tb_hosp_seller
                                (hosp_no, seller_id, bank_cd, deposit_no, depositor, etc, reg_aid, reg_dt)
                         VALUES 
                                (@HospNo, @SellerId, @BankCd, @DepositNo, @Depositor, @Etc, @RegAId, @RegDt);

                    SELECT LAST_INSERT_ID();
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<long>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("InsertTbHospSellerAsync: 관리자 셀러 등록 실패 {Exception}", e);
                return -1;
            }
        }

        private async Task<string?> GetHospSellerDepositorAsync(int hospSellerId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetHospSellerDepositorAsync by Id: [{Id}]", hospSellerId);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", hospSellerId, DbType.Int32);

                var sql = @"
                    SELECT depositor
                      FROM tb_hosp_seller
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<string>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("GetHospSellerDepositorAsync: 예금주 조회 실패 {Exception}", e);
                return null;
            }
        }

        private async Task<long> InsertTbHospSellerRemitAsync(InsertHospSellerRemitParams remitInfo, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("InsertTbHospSellerRemitAsync SellerId: [{SellerId}]", remitInfo.HospSellerId);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("HospSellerId", remitInfo.HospSellerId, DbType.Int32);
                parameters.Add("Amount", remitInfo.Amount, DbType.Int32);
                parameters.Add("VaMny", remitInfo.VaMny, DbType.Int32);
                parameters.Add("VaName", remitInfo.VaName, DbType.String);
                parameters.Add("Depositor", remitInfo.Depositor, DbType.String);
                parameters.Add("Etc", remitInfo.Etc, DbType.String);
                parameters.Add("RegAId", remitInfo.RegAId, DbType.String);
                parameters.Add("RegDt", (double)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DbType.Int64);

                var sql = @"
                    INSERT INTO tb_seller_remit
                                (hosp_seller_id, amount, va_mny, va_name, depositor, etc, reg_aid, reg_dt)
                         VALUES 
                                (@HospSellerId, @Amount, @VaMny, @VaName, @Depositor, @Etc, @RegAId, @RegDt);

                    SELECT LAST_INSERT_ID();
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<long>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("InsertTbHospSellerRemitAsync: 송금 요청 임시 등록 실패 {Exception}", e);
                return -1;
            }
        }

        private async Task<GetSellerRemitInfoByIdRow?> GetSellerRemitInfoByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetSellerRemitInfoByIdAsync by Id: [{Id}]", id);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);

                var sql = @"
                    SELECT trade_seq        AS TradeSeq,
                           trade_date       AS TradeDate,
                           res_cd           AS ResCd,
                           res_msg          AS ResMsg,
                           res_en_msg       AS ResEnMsg,
                           app_time         AS AppTime,
                           van_apptime      AS VanAppTime,
                           account          AS Account,
                           bank_cd          AS BankCode,
                           bank_name        AS BankName,
                           depositor        AS Depositor,
                           etc              AS Etc
                      FROM tb_seller_remit
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<GetSellerRemitInfoByIdRow>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("Error GetSellerRemitInfoByIdAsync: {Exception}", e);
                return null;
            }
        }

        private async Task<int> UpdateSellerRemitAsync(UpdateSellerRemitParams param, GetSellerRemitInfoByIdRow row, int id, string? etc, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UpdateSellerRemitAsync() {Id}", id);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                parameters.Add("TradeSeq", param.TradeSeq ?? row.TradeSeq, DbType.String);
                parameters.Add("TradeDate", param.TradeDate ?? row.TradeDate, DbType.String);
                parameters.Add("ResCd", param.ResCd ?? row.ResCd, DbType.String);
                parameters.Add("ResMsg", param.ResMsg ?? row.ResMsg, DbType.String);
                parameters.Add("ResEnMsg", param.ResEnMsg ?? row.ResEnMsg, DbType.String);
                parameters.Add("AppTime", param.AppTime ?? row.AppTime, DbType.String);
                parameters.Add("VanAppTime", param.VanAppTime ?? row.VanAppTime, DbType.String);
                parameters.Add("Account", param.Account ?? row.Account, DbType.String);
                parameters.Add("BankCode", param.BankCode ?? row.BankCode, DbType.String);
                parameters.Add("BankName", param.BankName ?? row.BankName, DbType.String);
                parameters.Add("Depositor", param.Depositor ?? row.Depositor, DbType.String);
                parameters.Add("Etc", etc ?? row.Etc, DbType.String);
                parameters.Add("Requested", "1", DbType.String);
                parameters.Add("ResDt", (double)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DbType.Int64);
                parameters.Add("BalAmount", (int)param.BalAmount, DbType.Int32);
                parameters.Add("Status", param.ResCd == "0000" ? "2" : "4", DbType.String);

                var sql = @"
                    UPDATE tb_seller_remit
                       SET trade_seq   = @TradeSeq,
                           trade_date  = @TradeDate,
                           res_cd      = @ResCd,
                           res_msg     = @ResMsg,
                           res_en_msg  = @ResEnMsg,
                           app_time    = @AppTime,
                           van_apptime = @VanAppTime,
                           account     = @Account,
                           bank_cd     = @BankCode,
                           bank_name   = @BankName,
                           depositor   = @Depositor,
                           etc         = @Etc,
                           requested   = @Requested,
                           res_dt      = @ResDt,
                           bal_amount  = @BalAmount,
                           status      = @Status
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("UpdateSellerRemitAsync 송금 결과 업데이트 실패 {Exception}", e);
                return -1;
            }
        }

        private async Task<string?> GetSellerRemitStatusByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetSellerRemitStatusByIdAsync by Id: [{Id}]", id);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);

                var sql = @"
                    SELECT status AS Status
                      FROM tb_seller_remit
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<string>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("Error GetSellerRemitStatusByIdAsync: {Exception}", e);
                return null;
            }
        }

        private async Task<int> UpdateSellerRemitAsync(int id, string? etc, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UpdateSellerRemitAsync() {Id}", id);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                parameters.Add("Status", "5", DbType.String);
                parameters.Add("Etc", etc, DbType.String);
                parameters.Add("RefreshDt", (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DbType.Int64);

                var sql = @"
                    UPDATE tb_seller_remit
                       SET status = @Status,
                           etc = @Etc,
                           refresh_dt = @RefreshDt
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("UpdateSellerRemitAsync 송금 결과 업데이트 실패 {Exception}", e);
                return -5;
            }
        }

        private async Task<long> GetHospSellerCountAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("GetHospSellerCountAsync by Id: [{Id}]", id);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);

                var sql = @"
                    SELECT COUNT(*)
                      FROM tb_hosp_seller
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteScalarAsync<long>(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("GetHospSellerCountAsync: 조회 실패 {Exception}", e);
                return 0;
            }
        }

        private async Task<int> UpdateSellerRemitEnabledAsync(int id, string enabled, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UpdateSellerRemitEnabledAsync() {Id}", id);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                parameters.Add("Enabled", enabled, DbType.String);

                var sql = @"
                    UPDATE tb_hosp_seller
                       SET enabled = @Enabled
                     WHERE id = @Id;
                ";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.ExecuteAsync(sql, parameters);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("UpdateSellerRemitEnabledAsync 관리자 셀러 업데이트(KCP 싱크) 실패 {Exception}", e);
                return -1;
            }
        }
        #endregion
    }
}
