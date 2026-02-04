using Dapper;
using Hello100Admin.BuildingBlocks.Common.Errors;
using System.Data;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateVisitPurposes;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.CreateVisitPurpose;
using Microsoft.Extensions.Configuration;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.UpdateVisitPurposeForNonNhisHealthScreening;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateCertificates;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.VisitPurpose
{
    public class VisitPurposeRepository : IVisitPurposeRepository
    {
        #region FIELD AREA ****************************************
        private readonly string _paperCheckUrl;
        private readonly IDbConnectionFactory _connection;
        private readonly ILogger<VisitPurposeRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public VisitPurposeRepository(IDbConnectionFactory connection, ILogger<VisitPurposeRepository> logger, IConfiguration config)
        {
            _connection = connection;
            _logger = logger;
            _paperCheckUrl = config["PaperCheckUrl"] ?? string.Empty;
        }
        #endregion

        #region IVISITPURPOSEREPOSITORY IMPLEMENTS METHOD AREA **************************************
        public async Task<int> BulkUpdateVisitPurposesAsync(BulkUpdateVisitPurposesCommand req, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("BulkUpdateVisitPurposesAsync HospKey: [{HospKey}]", req.HospKey);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("HospKey", req.HospKey, DbType.String);

                StringBuilder sbInsert = new StringBuilder();
                req.Items.ForEach(x =>
                {
                    sbInsert.AppendLine($",('{x.VpCd}', '{x.ShowYn}', {x.SortNo})");
                });

                #region == Query ==
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(" CREATE TEMPORARY TABLE hello100_api.tmp_purpose( ");
                sb.AppendLine("     vp_cd VARCHAR(100) NOT NULL     ");
                sb.AppendLine(" ,   show_yn CHAR(1) NOT NULL        ");
                sb.AppendLine(" ,   sort_no TINYINT(4) NOT NULL);   ");

                sb.AppendLine(" INSERT INTO hello100_api.tmp_purpose             ");
                sb.AppendLine("    (vp_cd, show_yn, sort_no)        ");
                sb.AppendLine("    VALUES                           ");
                sb.AppendLine(sbInsert.ToString().Substring(1, sbInsert.ToString().Length - 2) + " ; ");

                sb.AppendLine(" UPDATE tb_eghis_hosp_visit_purpose_info a");
                sb.AppendLine("  INNER JOIN hello100_api.tmp_purpose b                ");
                sb.AppendLine("     ON(b.vp_cd = a.vp_cd)                ");
                sb.AppendLine("     SET a.sort_no = b.sort_no            ");
                sb.AppendLine("     ,	a.show_yn = b.show_yn            ");
                sb.AppendLine("   WHERE a.hosp_key = @HospKey;           ");

                sb.AppendLine(" DROP TABLE hello100_api.tmp_purpose;     ");

                #endregion

                using var connection = _connection.CreateConnection();
                var result = await connection.ExecuteAsync(sb.ToString(), parameters);

                if (result < req.Items.Count)
                    throw new BizException(AdminErrorCode.VisitPurposeBulkUpdateFailed.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("BulkUpdateVisitPurposesAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }

        public async Task<int> CreateVisitPurposeApprovalAsync(DbSession db, string hospKey, string apprType, string data, string reqAId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("CreateVisitPurposeApprovalAsync HospKey: [{HospKey}]", hospKey);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ApprType", apprType, DbType.String);
                parameters.Add("ReqAId", reqAId, DbType.String);
                parameters.Add("ApprData", data.ToString(), DbType.String);
                parameters.Add("HospKey", hospKey, DbType.String);

                string query = @"
                    INSERT INTO tb_eghis_hosp_approval_info
                                ( appr_type, hosp_key, data, req_aid, reg_dt )
                         VALUES ( @ApprType, @HospKey, @ApprData, @ReqAId, UNIX_TIMESTAMP(NOW()) );
                    
                    SELECT IFNULL(LAST_INSERT_ID(), 0);
                ";

                var result = await db.ExecuteScalarAsync<int>(query, parameters, ct, _logger);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateVisitPurposeApprovalAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }

        public async Task<int> CreateVisitPurposeAsync(DbSession db, CreateVisitPurposeCommand req, int apprId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("CreateVisitPurposeAsync HospKey: [{HospKey}]", req.HospKey);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ParentCd", "0", DbType.String);
                parameters.Add("HospKey", req.HospKey, DbType.String);
                parameters.Add("InpuiryUrl", _paperCheckUrl, DbType.String);
                parameters.Add("InpuiryIdx", req.InpuiryIdx, DbType.Int32);
                parameters.Add("InpuirySkipYn", "Y", DbType.String);
                parameters.Add("Name", req.Name, DbType.String);
                parameters.Add("ShowYn", req.ShowYn, DbType.String);
                parameters.Add("SortNo", 0, DbType.Int16);
                parameters.Add("DelYn", "N", DbType.String);
                parameters.Add("ApprId", apprId, DbType.Int32);
                parameters.Add("AId", req.AId, DbType.String);
                parameters.Add("Role", req.Role, DbType.Int16);

                StringBuilder sbCondi = new StringBuilder();
                var i = 0;
                req.Details?.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        i++;
                        sbCondi.AppendLine(",   (   func_vp_cd_generator(@vp_cd, @HospKey)");
                        sbCondi.AppendLine(" 	,	@vp_cd                      ");
                        sbCondi.AppendLine(" 	,	@HospKey                    ");
                        sbCondi.AppendLine(" 	,	''                          ");
                        sbCondi.AppendLine($" 	,	'{x}'                       ");
                        sbCondi.AppendLine(" 	,	@ShowYn                     ");
                        sbCondi.AppendLine(" 	,	@Role                       ");
                        sbCondi.AppendLine($" 	,	0                           ");
                        sbCondi.AppendLine(" 	,	'N'                         ");
                        sbCondi.AppendLine(" 	,	UNIX_TIMESTAMP(NOW())	    ");
                        sbCondi.AppendLine(" 	)                               ");
                    }
                });

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                // 이지스병원내원목적정보 (최상위 부모 추가)
                sb.AppendLine(" SET @vp_cd:=func_vp_cd_generator('0', @HospKey) ;");
                sb.AppendLine(" INSERT INTO tb_eghis_hosp_visit_purpose_info ");
                sb.AppendLine("     (   vp_cd                   ");
                sb.AppendLine("     ,   parent_cd               ");
                sb.AppendLine("     ,   hosp_key                ");
                sb.AppendLine("     ,   inpuiry_url             ");
                sb.AppendLine("     ,   inpuiry_idx             ");
                sb.AppendLine("     ,   inpuiry_skip_yn         ");
                sb.AppendLine("     ,   name                    ");
                sb.AppendLine("     ,   show_yn                 ");
                sb.AppendLine("     ,   role                    ");
                sb.AppendLine("     ,   sort_no                 ");
                sb.AppendLine("     ,   del_yn                  ");
                sb.AppendLine("     ,   reg_dt                  ");
                sb.AppendLine(" 	)	                        ");
                sb.AppendLine(" 	VALUES	                    ");
                sb.AppendLine(" 	(   @vp_cd                  ");
                sb.AppendLine(" 	,	@ParentCd      	        ");
                sb.AppendLine(" 	,	@HospKey                ");
                sb.AppendLine(" 	,	@InpuiryUrl             ");
                sb.AppendLine(" 	,	@InpuiryIdx             ");
                sb.AppendLine(" 	,	@InpuirySkipYn          ");
                sb.AppendLine(" 	,	@Name                   ");
                sb.AppendLine(" 	,	@ShowYn                 ");
                sb.AppendLine(" 	,	@Role                   ");
                sb.AppendLine(" 	,	(SELECT IFNULL(MAX(z.sort_no),0) + 1 ");
                sb.AppendLine("            FROM tb_eghis_hosp_visit_purpose_info z ");
                sb.AppendLine("           WHERE z.hosp_key = @HospKey ");
                sb.AppendLine("             AND z.del_yn = 'N') ");
                sb.AppendLine(" 	,	@DelYn                  ");
                sb.AppendLine(" 	,	UNIX_TIMESTAMP(NOW())	");
                sb.AppendLine(" 	);                          ");

                if (!string.IsNullOrEmpty(sbCondi.ToString()))
                {
                    // 이지스병원내원목적정보 (자식(상세 항목) 추가)
                    sb.AppendLine(" INSERT INTO tb_eghis_hosp_visit_purpose_info ");
                    sb.AppendLine("     (   vp_cd                   ");
                    sb.AppendLine("     ,   parent_cd               ");
                    sb.AppendLine("     ,   hosp_key                ");
                    sb.AppendLine("     ,   inpuiry_url             ");
                    sb.AppendLine("     ,   name                    ");
                    sb.AppendLine("     ,   show_yn                 ");
                    sb.AppendLine("     ,   role                    ");
                    sb.AppendLine("     ,   sort_no                 ");
                    sb.AppendLine("     ,   del_yn                  ");
                    sb.AppendLine("     ,   reg_dt                  ");
                    sb.AppendLine(" 	)	                        ");
                    sb.AppendLine(" 	VALUES	                    ");
                    sb.AppendLine(sbCondi.ToString().Substring(1, sbCondi.ToString().Length - 2) + ";");
                }

                //정보요청내역 승인으로 처리
                sb.AppendLine("  UPDATE tb_eghis_hosp_approval_info         ");
                sb.AppendLine(" 	SET appr_yn = 'Y'                       ");
                sb.AppendLine(" 	 ,	 aid = @AId                         ");
                sb.AppendLine(" 	 ,	 appr_dt = UNIX_TIMESTAMP(NOW())    ");
                sb.AppendLine("  WHERE appr_id = @ApprId	;                ");

                #endregion


                var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

                if (result <= 0)
                    throw new BizException(AdminErrorCode.VisitPurposeCreateFailed.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateVisitPurposeAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }

        public async Task<int> UpdateVisitPurposeForNhisHealthScreeningAsync(string hospKey, string showYn, int role, List<string>? detailShowYn, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("CreateVisitPurposeApprovalAsync HospKey: [{HospKey}]", hospKey);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("HospKey", hospKey, DbType.String);
                parameters.Add("ShowYn", showYn, DbType.String);
                parameters.Add("Role", role, DbType.Int16);

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                StringBuilder condi = new StringBuilder();

                //상세내역 업데이트
                sb.AppendLine(" UPDATE tb_eghis_hosp_visit_purpose_info");
                sb.AppendLine("    SET show_yn = 'N'        ");
                sb.AppendLine("  WHERE parent_cd = '01'     ");
                sb.AppendLine("    AND hosp_key = @HospKey; ");

                //내원목적 상위 업데이트
                sb.AppendLine("  UPDATE tb_eghis_hosp_visit_purpose_info    ");
                sb.AppendLine("     SET show_yn = @ShowYn                   ");
                sb.AppendLine("     ,   role = @Role                        ");
                sb.AppendLine("  WHERE vp_cd= '01'                          ");
                sb.AppendLine("    AND hosp_key = @HospKey;                 ");

                if (detailShowYn != null)
                {
                    detailShowYn.ForEach(x =>
                    {
                        condi.Append($"'{x}',");
                    });

                    if (!string.IsNullOrEmpty(condi.ToString()))
                    {
                        sb.AppendLine(" UPDATE tb_eghis_hosp_visit_purpose_info");
                        sb.AppendLine("    SET show_yn = 'Y'        ");
                        sb.AppendLine("  WHERE parent_cd = '01'     ");
                        sb.AppendLine($"    AND vp_cd IN ({condi.ToString().Substring(0, condi.ToString().Length - 1)}) ");
                        sb.AppendLine("    AND hosp_key = @HospKey; ");
                    }
                }
                #endregion

                using var connection = _connection.CreateConnection();
                var result = await connection.ExecuteAsync(sb.ToString(), parameters);

                if (result <= 0)
                    throw new BizException(AdminErrorCode.VisitPurposeUpdateFailed.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("CreateVisitPurposeApprovalAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }

        public async Task<int> UpdateVisitPurposeForNonNhisHealthScreeningAsync(DbSession db, UpdateVisitPurposeForNonNhisHealthScreeningCommand req, int apprId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("UpdateVisitPurposeForNonNhisHealthScreeningAsync HospKey: [{HospKey}]", req.HospKey);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("VpCd", req.VpCd, DbType.String);
                parameters.Add("HospKey", req.HospKey, DbType.String);
                parameters.Add("InpuiryUrl", _paperCheckUrl, DbType.String);
                parameters.Add("InpuiryIdx", req.InpuiryIdx, DbType.Int32);
                parameters.Add("InpuirySkipYn", "Y", DbType.String);
                parameters.Add("Name", req.Name, DbType.String);
                parameters.Add("ShowYn", req.ShowYn, DbType.String);
                parameters.Add("ApprId", apprId, DbType.Int32);
                parameters.Add("AId", req.AId, DbType.String);
                parameters.Add("Role", req.Role, DbType.Int16);

                StringBuilder sbCondi = new StringBuilder();
                var i = 0;
                req.Details?.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        i++;
                        sbCondi.AppendLine(",   (   func_vp_cd_generator(@VpCd, @HospKey) ");
                        sbCondi.AppendLine(" 	,	@VpCd                       ");
                        sbCondi.AppendLine(" 	,	@HospKey                    ");
                        sbCondi.AppendLine(" 	,	''                          ");
                        sbCondi.AppendLine($" 	,	'{x}'                       ");
                        sbCondi.AppendLine(" 	,	@ShowYn                     ");
                        sbCondi.AppendLine(" 	,	@Role                       ");
                        sbCondi.AppendLine($" 	,	{i}                         ");
                        sbCondi.AppendLine(" 	,	'N'                         ");
                        sbCondi.AppendLine(" 	,	UNIX_TIMESTAMP(NOW())	    ");
                        sbCondi.AppendLine(" 	)                               ");
                    }
                });

                #region == Query ==
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("  UPDATE tb_eghis_hosp_visit_purpose_info");
                sb.AppendLine("     SET inpuiry_url     = @InpuiryUrl     ");
                sb.AppendLine("     ,   inpuiry_idx     = @InpuiryIdx     ");
                sb.AppendLine("     ,   inpuiry_skip_yn = @InpuirySkipYn  ");
                sb.AppendLine("     ,   name            = @Name           ");
                sb.AppendLine("     ,   show_yn         = @ShowYn         ");
                sb.AppendLine("     ,   role            = @Role           ");
                sb.AppendLine("   WHERE vp_cd           = @VpCd           ");
                sb.AppendLine(" 	AND	hosp_key       =  @HospKey    ;   ");

                if (!string.IsNullOrEmpty(sbCondi.ToString()) || req.DetailYn == "Y")
                {
                    sb.AppendLine("  DELETE FROM tb_eghis_hosp_visit_purpose_info");
                    sb.AppendLine("   WHERE parent_cd  = @VpCd           ");
                    sb.AppendLine(" 	AND	hosp_key   = @HospKey   ;    ");

                    if (req.DetailYn == "Y")
                    {
                        sb.AppendLine(" INSERT INTO tb_eghis_hosp_visit_purpose_info ");
                        sb.AppendLine("     (   vp_cd                   ");
                        sb.AppendLine("     ,   parent_cd               ");
                        sb.AppendLine("     ,   hosp_key                ");
                        sb.AppendLine("     ,   inpuiry_url             ");
                        sb.AppendLine("     ,   name                    ");
                        sb.AppendLine("     ,   show_yn                 ");
                        sb.AppendLine("     ,   role                    ");
                        sb.AppendLine("     ,   sort_no                 ");
                        sb.AppendLine("     ,   del_yn                  ");
                        sb.AppendLine("     ,   reg_dt                  ");
                        sb.AppendLine(" 	)	                        ");
                        sb.AppendLine(" 	VALUES	                    ");
                        sb.AppendLine(sbCondi.ToString().Substring(1, sbCondi.ToString().Length - 2) + ";");
                    }
                }

                //정보요청내역 승인으로 처리
                sb.AppendLine("  UPDATE tb_eghis_hosp_approval_info         ");
                sb.AppendLine(" 	SET appr_yn = 'Y'                       ");
                sb.AppendLine(" 	 ,	 aid = @AId                         ");
                sb.AppendLine(" 	 ,	 appr_dt = UNIX_TIMESTAMP(NOW())    ");
                sb.AppendLine("  WHERE appr_id = @ApprId	;                ");
                #endregion

                var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

                if (result <= 0)
                    throw new BizException(AdminErrorCode.VisitPurposeUpdateFailed.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("UpdateVisitPurposeForNonNhisHealthScreeningAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }

        public async Task<int> DeleteVisitPurposeAsync(string vpCd, string hospKey, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("DeleteVisitPurposeAsync HospKey: [{HospKey}]", hospKey);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("VpCd", vpCd, DbType.String);
                parameters.Add("HospKey", hospKey, DbType.String);

                string query = @"
                    UPDATE tb_eghis_hosp_visit_purpose_info
                       SET del_yn = 'Y'
                     WHERE vp_cd = @VpCd
                       AND hosp_key = @HospKey
                ";

                using var connection = _connection.CreateConnection();
                var result = await connection.ExecuteAsync(query, parameters);

                if (result <= 0)
                    throw new BizException(AdminErrorCode.VisitPurposeDeleteFailed.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteVisitPurposeAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }

        public async Task<int> BulkUpdateCertificatesAsync(BulkUpdateCertificatesCommand req, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("BulkUpdateVisitPurposesAsync HospKey: [{HospKey}]", req.HospKey);

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("HospKey", req.HospKey, DbType.String);

                StringBuilder sbInsert = new StringBuilder();
                req.Items.ForEach(x =>
                {
                    sbInsert.AppendLine($",('{x.ReDocCd}', '{x.ShowYn}', {x.SortNo})");
                });

                #region == Query ==
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(" CREATE TEMPORARY TABLE hello100_api.tmp_document(");
                sb.AppendLine("     re_doc_cd CHAR(2) NOT NULL      ");
                sb.AppendLine(" ,   show_yn CHAR(1) NOT NULL        ");
                sb.AppendLine(" ,   sort_no TINYINT(4) NOT NULL);   ");

                sb.AppendLine(" INSERT INTO hello100_api.tmp_document            ");
                sb.AppendLine("    (re_doc_cd, show_yn, sort_no)    ");
                sb.AppendLine("    VALUES                           ");
                sb.AppendLine(sbInsert.ToString().Substring(1, sbInsert.ToString().Length - 2) + " ; ");

                sb.AppendLine(" UPDATE tb_eghis_recert_doc_info a   ");
                sb.AppendLine("  INNER JOIN hello100_api.tmp_document b          ");
                sb.AppendLine("     ON(b.re_doc_cd = a.re_doc_cd)   ");
                sb.AppendLine("     SET a.sort_no = b.sort_no       ");
                sb.AppendLine("     ,	a.show_yn = b.show_yn       ");
                sb.AppendLine("   WHERE a.hosp_key = @HospKey;      ");

                sb.AppendLine(" DROP TABLE hello100_api.tmp_document;     ");
                #endregion

                using var connection = _connection.CreateConnection();
                var result = await connection.ExecuteAsync(sb.ToString(), parameters);

                if (result < req.Items.Count)
                    throw new BizException(AdminErrorCode.CertificateBulkUpdateFailed.ToError());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("BulkUpdateVisitPurposesAsync {Exception}", e);
                throw new BizException(GlobalErrorCode.DataInsertError.ToError());
            }
        }
        #endregion
    }
}
