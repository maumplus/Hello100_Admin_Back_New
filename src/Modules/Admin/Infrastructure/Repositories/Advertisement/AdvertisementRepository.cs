using Dapper;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Advertisement
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<AdvertisementRepository> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public AdvertisementRepository(ILogger<AdvertisementRepository> logger, IConfiguration config)
        {
            _logger = logger;
        }
        #endregion

        #region IADVERTISEMENTREPOSITORY IMPLEMENTS AREA **********************************
        public async Task<int> CreateAdvertisementAsync(DbSession db, TbAdInfoEntity adInfo, TbImageInfoEntity imageInfo, CancellationToken ct)
        {
            var startDt = !string.IsNullOrEmpty(adInfo.StartDt) ? Convert.ToDateTime(adInfo.StartDt).ToString("yyyyMMdd") : string.Empty;
            var endDt = !string.IsNullOrEmpty(adInfo.EndDt) ? Convert.ToDateTime(adInfo.EndDt).ToString("yyyyMMdd") : string.Empty;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("AdType", adInfo.AdType, DbType.String);
            parameters.Add("HospKey", string.Empty, DbType.String); // 빈값 의도
            parameters.Add("MdCd", adInfo.MdCd ?? string.Empty, DbType.String);
            parameters.Add("ShowYn", adInfo.ShowYn, DbType.String);
            parameters.Add("SendType", adInfo.SendType, DbType.String);
            parameters.Add("Url", adInfo.Url, DbType.String);
            parameters.Add("StartDt", startDt, DbType.String);
            parameters.Add("EndDt", endDt, DbType.String);
            parameters.Add("SortNo", adInfo.SortNo, DbType.Int16);
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);
            parameters.Add("ImgUrl", imageInfo.Url ?? string.Empty, DbType.String);
            parameters.Add("LinkType", adInfo.LinkType ?? "N", DbType.String);
            parameters.Add("Url2", adInfo.Url2 ?? "", DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();

            if (adInfo.AdType == AdvertType.BA.ToString())
            {
                sb.AppendLine("  UPDATE tb_ad_info              ");
                sb.AppendLine("  	SET sort_no = sort_no + 1   ");
                sb.AppendLine("   WHERE ad_type = @AdType       ");
                sb.AppendLine("     AND del_yn = 'N'            ");
                sb.AppendLine("     AND sort_no >= @SortNo;     ");
            }

            sb.AppendLine(" INSERT INTO tb_ad_info          ");
            sb.AppendLine(" 	(	ad_type	                ");
            sb.AppendLine(" 	,	hosp_key	            ");
            sb.AppendLine(" 	,	md_cd	                ");
            sb.AppendLine(" 	,	show_yn	                ");
            sb.AppendLine(" 	,	url	                    ");
            sb.AppendLine(" 	,	send_type               ");
            sb.AppendLine(" 	,	link_type               ");
            sb.AppendLine(" 	,	start_dt                ");
            sb.AppendLine(" 	,	end_dt                  ");
            sb.AppendLine(" 	,	sort_no	                ");
            sb.AppendLine(" 	,	del_yn	                ");
            sb.AppendLine(" 	,	reg_dt	                ");
            sb.AppendLine(" 	,	url2	                ");
            sb.AppendLine(" 	)	                        ");
            sb.AppendLine(" 	VALUES	                    ");
            sb.AppendLine(" 	(   @AdType                 ");
            sb.AppendLine(" 	,	@HospKey	            ");
            sb.AppendLine(" 	,	@MdCd	                ");
            sb.AppendLine(" 	,	@ShowYn	                ");
            sb.AppendLine(" 	,	@Url	                ");
            sb.AppendLine(" 	,	@SendType               ");
            sb.AppendLine(" 	,	@LinkType               ");
            sb.AppendLine(" 	,	@StartDt                ");
            sb.AppendLine(" 	,	@EndDt                  ");
            sb.AppendLine(" 	,	@SortNo	                ");
            sb.AppendLine(" 	,	'N'	                    ");
            sb.AppendLine(" 	,	UNIX_TIMESTAMP(NOW()) 	");
            sb.AppendLine(" 	,   @Url2 	");
            sb.AppendLine(" 	);                          ");

            if (string.IsNullOrWhiteSpace(imageInfo.Url) == false)
            {
                sb.AppendLine(" SET @idx:= (SELECT LAST_INSERT_ID()) ; ");
                sb.AppendLine(" INSERT INTO tb_image_info	    ");
                sb.AppendLine(" 	(	img_key                 ");
                sb.AppendLine(" 	,	url                     ");
                sb.AppendLine(" 	,	del_yn	                ");
                sb.AppendLine(" 	,	reg_dt                  ");
                sb.AppendLine(" 	)                           ");
                sb.AppendLine("   VALUES                        ");
                sb.AppendLine(" 	(	func_HMACSHA256( @EncKey, CONCAT('ad', @idx, @AdType))  ");
                sb.AppendLine(" 	,	@ImgUrl                 ");
                sb.AppendLine(" 	,	'N'	                    ");
                sb.AppendLine(" 	,	UNIX_TIMESTAMP(NOW())   ");
                sb.AppendLine(" 	);                          ");
            }
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.CreateAdvertisementFailed.ToError());

            return result;
        }

        public async Task<int> UpdatePopupAsync(DbSession db, TbAdInfoEntity adInfo, TbImageInfoEntity imageInfo, CancellationToken ct)
        {
            var startDt = !string.IsNullOrEmpty(adInfo.StartDt) ? Convert.ToDateTime(adInfo.StartDt).ToString("yyyyMMdd") : string.Empty;
            var endDt = !string.IsNullOrEmpty(adInfo.EndDt) ? Convert.ToDateTime(adInfo.EndDt).ToString("yyyyMMdd") : string.Empty;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("AdId", adInfo.AdId, DbType.Int32);
            parameters.Add("ShowYn", adInfo.ShowYn, DbType.String);
            parameters.Add("SendType", adInfo.SendType, DbType.String);
            parameters.Add("Url", adInfo.Url, DbType.String);
            parameters.Add("StartDt", startDt, DbType.String);
            parameters.Add("EndDt", endDt, DbType.String);
            parameters.Add("ImgId", imageInfo.ImgId, DbType.Int32);
            parameters.Add("ImgUrl", imageInfo.Url ?? string.Empty, DbType.String);
            parameters.Add("LinkType", adInfo.LinkType ?? "N", DbType.String);
            parameters.Add("Url2", adInfo.Url2 ?? "", DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("  UPDATE tb_ad_info              ");
            sb.AppendLine(" 	SET	show_yn	    = @ShowYn	");
            sb.AppendLine(" 	,	url	        = @Url	    ");
            sb.AppendLine(" 	,	link_type   = @LinkType ");
            sb.AppendLine(" 	,	start_dt    = @StartDt  ");
            sb.AppendLine(" 	,	end_dt      = @EndDt    ");
            sb.AppendLine("     ,   send_type   = @SendType ");
            sb.AppendLine("     ,   url2   = @Url2 ");
            sb.AppendLine("   WHERE ad_id = @AdId;          ");

            if (string.IsNullOrWhiteSpace(imageInfo.Url) == false)
            {
                sb.AppendLine("  UPDATE tb_image_info	        ");
                sb.AppendLine(" 	SET url = @ImgUrl           ");
                sb.AppendLine("   WHERE img_id = @ImgId;        ");
            }
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpdatePopupAdvertisementFailed.ToError());

            return result;
        }

        public async Task<int> DeleteAdvertisementAsync(DbSession db, TbAdInfoEntity adInfo, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("AdId", adInfo.AdId, DbType.Int32);
            parameters.Add("AdType", adInfo.AdType, DbType.String);
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);

            var query = @"
                UPDATE tb_ad_info
                   SET del_yn = 'Y'
                 WHERE ad_id = @AdId
                   AND ad_type = @AdType;

                UPDATE tb_image_info
                   SET del_yn = 'Y'
                 WHERE img_key = func_HMACSHA256( @EncKey, CONCAT('ad', @AdId, @AdType));
            ";

            var result = await db.ExecuteAsync(query, parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.DeleteAdvertisementFailed.ToError());

            return result;
        }

        public async Task<int> BulkUpdateEghisBannersAsync(DbSession db, List<TbAdInfoEntity> adInfo, CancellationToken ct)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("AdType", AdvertType.BA.ToString(), DbType.String);

            StringBuilder sbTmp = new StringBuilder();

            adInfo.ForEach(x =>
            {
                sbTmp.AppendLine($",({x.AdId}, {x.SortNo}, '{x.ShowYn}')");
            });

            #region == Query ==
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(" CREATE TEMPORARY TABLE hello100_api.tmp_advert_sort");
            sb.AppendLine("     (   ad_id INT(11) NOT NULL ");
            sb.AppendLine("	    ,   sort_no TINYINT(4) NOT NULL DEFAULT '0'");
            sb.AppendLine("	    ,   show_yn CHAR(1) NOT NULL DEFAULT 'Y'");
            sb.AppendLine("     );");

            sb.AppendLine(" INSERT INTO hello100_api.tmp_advert_sort");
            sb.AppendLine("	    (	ad_id       ");
            sb.AppendLine("	    , 	sort_no     ");
            sb.AppendLine("	    , 	show_yn     ");
            sb.AppendLine("	    )               ");
            sb.AppendLine("  VALUES             ");
            sb.AppendLine($"{sbTmp.ToString(1, sbTmp.Length - 2)} ;");

            sb.AppendLine("  UPDATE tb_ad_info a            ");
            sb.AppendLine("   INNER JOIN hello100_api.tmp_advert_sort b  ");
            sb.AppendLine("     ON (b.ad_id = a.ad_id)      ");
            sb.AppendLine("     SET a.sort_no = b.sort_no   ");
            sb.AppendLine("     ,   a.show_yn = b.show_yn   ");
            sb.AppendLine("   WHERE a.ad_type = @AdType;    ");
            sb.AppendLine(" DROP TABLE hello100_api.tmp_advert_sort;     ");

            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.EghisBannerBulkUpdateFailed.ToError());

            return result;
        }

        public async Task<int> UpdateEghisBannerAsync(DbSession db, TbAdInfoEntity adInfo, TbImageInfoEntity imageInfo, CancellationToken ct)
        {
            var startDt = !string.IsNullOrEmpty(adInfo.StartDt) ? Convert.ToDateTime(adInfo.StartDt).ToString("yyyyMMdd") : string.Empty;
            var endDt = !string.IsNullOrEmpty(adInfo.EndDt) ? Convert.ToDateTime(adInfo.EndDt).ToString("yyyyMMdd") : string.Empty;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("AdId", adInfo.AdId, DbType.Int32);
            parameters.Add("ShowYn", adInfo.ShowYn, DbType.String);
            parameters.Add("SendType", adInfo.SendType, DbType.String);
            parameters.Add("AdType", adInfo.AdType, DbType.String);
            parameters.Add("Url", adInfo.Url, DbType.String);
            parameters.Add("StartDt", startDt, DbType.String);
            parameters.Add("EndDt", endDt, DbType.String);
            parameters.Add("SortNo", adInfo.SortNo, DbType.Int16);
            parameters.Add("ImgId", imageInfo.ImgId, DbType.Int32);
            parameters.Add("ImgUrl", imageInfo.Url ?? string.Empty, DbType.String);
            parameters.Add("LinkType", adInfo.LinkType ?? "N", DbType.String);
            parameters.Add("Url2", adInfo.Url2 ?? "", DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(" SET @oldSortNo:= (SELECT IFNULL(sort_no, 9999) FROM tb_ad_info WHERE ad_id = @AdId and ad_type= @AdType ); ");

            sb.AppendLine("  UPDATE tb_ad_info                              ");
            sb.AppendLine("     SET sort_no = sort_no -1                    ");
            sb.AppendLine("   WHERE ad_id <> @AdId                          ");
            sb.AppendLine("     AND sort_no BETWEEN @oldSortNo AND @SortNo  ");
            sb.AppendLine("     AND @oldSortNo < @SortNo ;                 ");

            sb.AppendLine("  UPDATE tb_ad_info                              ");
            sb.AppendLine("     SET sort_no = sort_no +1                    ");
            sb.AppendLine("   WHERE ad_id <> @AdId                          ");
            sb.AppendLine("     AND sort_no BETWEEN @oldSortNo AND @SortNo  ");
            sb.AppendLine("     AND @oldSortNo > @SortNo ;                 ");

            sb.AppendLine("  UPDATE tb_ad_info              ");
            sb.AppendLine(" 	SET	show_yn	    = @ShowYn	");
            sb.AppendLine(" 	,	url	        = @Url	    ");
            sb.AppendLine(" 	,	link_type   = @LinkType ");
            sb.AppendLine(" 	,	start_dt    = @StartDt  ");
            sb.AppendLine(" 	,	end_dt      = @EndDt    ");
            sb.AppendLine("     ,   sort_no     = @SortNo   ");
            sb.AppendLine("     ,   send_type   = @SendType ");
            sb.AppendLine("     ,   url2   = @Url2 ");
            sb.AppendLine("   WHERE ad_id = @AdId;          ");

            if (string.IsNullOrWhiteSpace(imageInfo.Url) == false)
            {
                sb.AppendLine("  UPDATE tb_image_info	        ");
                sb.AppendLine(" 	SET url = @ImgUrl           ");
                sb.AppendLine("   WHERE img_id = @ImgId;        ");
            }
            #endregion

            var result = await db.ExecuteAsync(sb.ToString(), parameters, ct, _logger);

            if (result <= 0)
                throw new BizException(AdminErrorCode.UpdateEghisBannerAdvertisementFailed.ToError());

            return result;
        }
        #endregion
    }
}
