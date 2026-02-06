using DocumentFormat.OpenXml.Drawing;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results
{
    public class GetRequestsResult
    {
        public int RowNum { get; set; }
        public string HospKey { get; set; } = default!;
        public Int64 ApprId { get; set; }
        public string ApprType { get; set; } = default!;
        public object Data { get; set; } = default!;
        public string Aid { get; set; } = default!;
        public string AIdName { get; set; } = default!;
        public string ReqAid { get; set; } = default!;
        public char ApprYn { get; set; }
        public string ApprDt { get; set; } = default!;
        public string ReqDt { get; set; } = default!;
        public string HospClsCd { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Addr { get; set; } = default!;
        public string PostCd { get; set; } = default!;
        public string Tel { get; set; } = default!;
        public string Site { get; set; } = default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string RegDt { get; set; } = default!;

        public string CatType { get; set; } = default!;
        public string CatTypeNm { get; set; } = default!;
    }

    public class GetRequestBugsResult
    {
        public long RowNum { get; set; }
        /// <summary>
        /// 레거시 TranId 역할 포함
        /// </summary>
        public long HpId { get; set; }
        public string HospKey { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ApprName { get; set; } = default!;
        public string RegDt { get; set; } = default!;
        public string ApprDt { get; set; } = default!;
        public string ApprAid { get; set; } = default!;
        public string ApprYn { get; set; } = default!;
        public string HospNo { get; set; } = default!;
        public string HospAddr { get; set; } = default!;
    }

    public class GetRequestBugResult
    {        
        public long HpId { get; set; }
        public string HospKey { get; set; } = default!;
        public string Msg { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ProposalType { get; set; } = default!;
        public string ProposalName { get; set; } = default!;
        public string RegDt { get; set; } = default!;
        public string ApprDt { get; set; } = default!;
        public string ApprAid { get; set; } = default!;
        public string HospNo { get; set; } = default!;
        public string HospAddr { get; set; } = default!;
    }
}
