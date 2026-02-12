using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using System.Text;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.Keywords.Results;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Keywords
{
    public class KeywordsStore : IKeywordsStore
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<KeywordsStore> _logger;
        #endregion

        #region CONSTRUCTOR AREA *******************************************
        public KeywordsStore(ILogger<KeywordsStore> logger)
        {
            _logger = logger;
        }
        #endregion

        #region IKEYWORDSSTORE IMPLEMENTS METHOD AREA **************************************
        public async Task<List<GetKeywordsResult>> GetKeywordsAsync(DbSession db, string? keyword, string? masterSeq, CancellationToken ct = default)
        {
            // 파라미터 사용 안함
            //var parameters = new DynamicParameters();
            //parameters.Add("@Keyword", keyword, DbType.String);

            #region == Query ==
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("   select	");
            sb.AppendLine("      CONCAT(master_seq,'_',detail_seq) as Kid,   	");
            sb.AppendLine("      master_seq as MasterSeq,   	");
            sb.AppendLine("      detail_seq as DetailSeq,	");
            sb.AppendLine("      detail_name as Keyword,	");
            sb.AppendLine("      sort_no as SortNo,	");
            sb.AppendLine("      search_cnt as SearchCnt,	");
            sb.AppendLine("      'N' as DelYn	");
            sb.AppendLine("   from	");
            sb.AppendLine("      hello100.tb_keyword_detail tkd 	");
            sb.AppendLine("   where	 ");
            sb.AppendLine("   (select detail_use_yn from hello100.tb_keyword_master tkm where tkm.master_seq=tkd.master_seq)!='N' ");
            if (!string.IsNullOrEmpty(masterSeq))
            {
                sb.AppendLine("     and master_seq=" + masterSeq + " 	");
            }
            if (keyword != "")
            {
                sb.AppendLine("     and detail_name like '%" + keyword + "%'");
            }

            sb.AppendLine("     union	");
            sb.AppendLine("     select	");
            sb.AppendLine("        CONCAT(master_seq,'_',0) as Kid,     	");
            sb.AppendLine("        master_seq as MasterSeq,      	");
            sb.AppendLine("        0 as DetailSeq,  	");
            sb.AppendLine("        concat('[대표] ',master_name) as Keyword,      	");
            sb.AppendLine("        sort_no as SortNo,    	");
            sb.AppendLine("        search_cnt as SearchCnt,   	");
            sb.AppendLine("        'N' as DelYn	");
            sb.AppendLine("     from	");
            sb.AppendLine("        hello100.tb_keyword_master tkm 	");
            sb.AppendLine("   where	 show_yn='Y' ");
            if (!string.IsNullOrEmpty(masterSeq))
            {
                sb.AppendLine("     and master_seq=" + masterSeq + " 	");
            }
            if (keyword != "")
            {
                sb.AppendLine("     and master_name like '%" + keyword + "%'");
            }

            sb.AppendLine("        order by MasterSeq asc ,DetailSeq asc  	");
            #endregion

            var result = (await db.QueryAsync<GetKeywordsResult>(sb.ToString(), ct: ct, logger: _logger)).ToList();

            return result;
        }
        #endregion
    }
}
