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

    public class GetRequestUntactsResult
    {
        /// <summary>
        /// TranSeq 대체
        /// </summary>
        public int Seq { get; set; }
        public int RowNum { get; set; }
        public string HospNo { get; set; } = default!;
        public string HospNm { get; set; } = default!;
        public string HospKey { get; set; } = default!;
        public string DoctNm { get; set; } = default!;
        public string DoctTel { get; set; } = default!;
        public string RegDt { get; set; } = default!;
        public string ModDt { get; set; } = default!;
        public string JoinState { get; set; } = default!;
        public string JoinStateNm { get; set; } = default!;

        /*
        public string HospKey { get; set; }
        public string JoinStateModDt { get; set; }
        public string ReturnReason { get; set; }
        */
    }

    public class GetRequestUntactResult
    {
        public string HospNm { get; set; } = default!;
        public string HospNo { get; set; } = default!;
        public string HospKey { get; set; } = default!;
        public string EmplNo { get; set; } = default!;
        public string HospTel { get; set; } = default!;
        public string PostCd { get; set; } = default!;
        public string Addr { get; set; } = default!;
        public string DoctNo { get; set; } = default!;
        public string DoctNoType { get; set; } = default!;
        public string DoctNoTypeNm { get; set; } = default!;
        public int DoctLicenseFileSeq { get; set; }
        public string DoctLicenseFileSeqNm { get; set; } = default!;
        public string DoctNm { get; set; } = default!;
        public string DoctBirthday { get; set; } = default!;
        public string DoctTel { get; set; } = default!;
        public string DoctIntro { get; set; } = default!;
        public int DoctFileSeq { get; set; }
        public string DoctFileSeqNm { get; set; } = default!;
        public string DoctHistory { get; set; } = default!;
        public string ClinicTime { get; set; } = default!;
        public string ClinicGuide { get; set; } = default!;
        public int AccountInfoFileSeq { get; set; }
        public int BusinessFileSeq { get; set; }
        public string JoinState { get; set; } = default!;
        public string RegDt { get; set; } = default!;
        public string DoctFilePath { get; set; } = default!;
        public string BusinessFilePath { get; set; } = default!;
        public string LicenseFilePath { get; set; } = default!;
        public string AccountFilePath { get; set; } = default!;
        public string JoinStateNm { get; set; } = default!;
        public string ReturnReason { get; set; } = default!;
    }
}
