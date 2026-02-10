using Dapper;
using System.Data;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results;
using Microsoft.Extensions.Logging;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Advertisement
{
    public class AdvertisementStore : IAdvertisementStore
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<AdvertisementStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public AdvertisementStore(ILogger<AdvertisementStore> logger)
        {
            _logger = logger;
        }
        #endregion

        #region IADVERTISEMENTSTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<ListResult<GetPopupsResult>> GetPopupsAsync(DbSession db, int pageNo, int pageSize, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Limit", pageSize * pageNo, DbType.Int32);
            parameters.Add("OffSet", (pageNo - 1) * pageSize, DbType.Int32);
            parameters.Add("AdType", ImageUploadType.PO.ToString(), DbType.String); // AdvertType.PO
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);

            StringBuilder sbCondi = new StringBuilder();

            sbCondi.AppendLine("  WHERE a.ad_type = @AdType   ");
            sbCondi.AppendLine("    AND a.del_yn = 'N'        ");

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SET @total_cnt:= (SELECT COUNT(*) FROM VM_ADVERTS a {sbCondi.ToString()});");
            sb.AppendLine($"SET @rownum:= (@total_cnt +1) - @OffSet;");
            sb.AppendLine("  SELECT @rownum:= @rownum - 1 AS RowNum  ");
            sb.AppendLine("	     ,	a.ad_id		AS AdId     ");
            sb.AppendLine("	     ,	a.ad_type 	AS AdType   ");
            sb.AppendLine("	     ,	a.hosp_key	AS HospKey  ");
            sb.AppendLine("	     ,	a.md_cd   	AS MdCd     ");
            sb.AppendLine("	     ,	a.show_yn 	AS ShowYn   ");
            sb.AppendLine("	     ,	a.send_type 	AS SendType ");
            sb.AppendLine("	     ,	a.url     	AS Url      ");
            sb.AppendLine("	     ,	a.link_type	AS LinkType      ");
            sb.AppendLine("	     ,	a.sort_no 	AS SortNo   ");
            sb.AppendLine("	     ,	a.del_yn  	AS DelYn    ");
            sb.AppendLine("	     ,	DATE_FORMAT(a.start_dt, '%Y-%m-%d')	AS StartDt");
            sb.AppendLine("	     ,	DATE_FORMAT(a.end_dt, '%Y-%m-%d')	AS EndDt");
            sb.AppendLine("	     ,	TIMESTAMPDIFF(day, NOW(), IFNULL(a.end_dt,'')) AS CntDt");
            sb.AppendLine("	     ,	from_unixtime(a.reg_dt, '%Y-%m-%d %H:%i')  AS RegDt");
            sb.AppendLine("      ,  a.img_url      AS ImgUrl");
            sb.AppendLine("      ,  b.img_id      AS ImgId");
            sb.AppendLine("	   FROM VM_ADVERTS a             ");
            sb.AppendLine("    LEFT JOIN tb_image_info b    ");
            sb.AppendLine("     ON (b.img_key = func_HMACSHA256(@EncKey, CONCAT('ad', a.ad_id, a.ad_type)) ");
            sb.AppendLine("     AND b.del_yn = 'N')         ");
            sb.AppendLine(sbCondi.ToString());
            sb.AppendLine("   ORDER BY a.reg_dt desc  ");
            sb.AppendLine("   LIMIT @OffSet, @Limit;");

            sb.AppendLine($"SELECT @total_cnt;");

            #endregion

            var multi = await db.QueryMultipleAsync(sb.ToString(), parameters, ct, _logger);

            var result = new ListResult<GetPopupsResult>();

            result.Items = (await multi.ReadAsync<GetPopupsResult>()).ToList();
            result.TotalCount = await multi.ReadSingleAsync<int>();

            return result;
        }

        public async Task<GetPopupResult> GetPopupAsync(DbSession db, int popupId, CancellationToken ct)
        {
            var parameters = new DynamicParameters();
            parameters.Add("AdId", popupId, DbType.Int32);
            parameters.Add("EncKey", "d3fa7fa7873c38097b31feb7bcd1c017ff222aee", DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  SELECT a.ad_id		AS AdId     ");
            sb.AppendLine("     ,	a.ad_type 	AS AdType   ");
            sb.AppendLine("     ,	a.send_type AS SendType ");
            sb.AppendLine("     ,	a.hosp_key	AS HospKey  ");
            sb.AppendLine("     ,	a.md_cd   	AS MdCd     ");
            sb.AppendLine("     ,	a.show_yn 	AS ShowYn   ");
            sb.AppendLine("     ,	a.url     	AS Url      ");
            sb.AppendLine("     ,	a.url2     	AS Url2      ");
            sb.AppendLine("	     ,	a.link_type	AS LinkType      ");
            sb.AppendLine("     ,	a.sort_no 	AS SortNo   ");
            sb.AppendLine("     ,	a.del_yn  	AS DelYn    ");
            sb.AppendLine("     ,	DATE_FORMAT(IFNULL(a.start_dt,''), '%Y-%m-%d')	AS StartDt  ");
            sb.AppendLine("     ,	DATE_FORMAT(IFNULL(a.end_dt,'')  , '%Y-%m-%d')	AS EndDt    ");
            sb.AppendLine("     ,	from_unixtime(a.reg_dt, '%Y-%m-%d %H:%i')  	AS RegDt    ");
            sb.AppendLine("     ,   b.img_id    AS ImgId    ");
            sb.AppendLine("     ,   b.img_key   AS ImgKey   ");
            sb.AppendLine("     ,   b.url       AS ImgUrl   ");
            sb.AppendLine("    FROM tb_ad_info  a           ");
            sb.AppendLine("    LEFT JOIN tb_image_info b    ");
            sb.AppendLine("     ON (b.img_key = func_HMACSHA256(@EncKey, CONCAT('ad', a.ad_id, a.ad_type)) ");
            sb.AppendLine("     AND b.del_yn = 'N')         ");
            sb.AppendLine("   WHERE a.ad_id = @AdId         ");
            sb.AppendLine("     AND a.del_yn = 'N'          ");
            #endregion

            var result = await db.QueryFirstOrDefaultAsync<GetPopupResult>(sb.ToString(), parameters, ct, _logger);

            if (result == null)
                throw new BizException(AdminErrorCode.PopupAdvertisementNotFound.ToError());

            return result;
        }
        #endregion
    }
}
