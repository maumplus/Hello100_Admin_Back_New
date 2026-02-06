using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbHospitalProposalInfoEntity
    {
        /// <summary>
        /// 제안아이디
        /// </summary>
        public int Hpid { get; set; }
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HopsKey { get; set; } = null!;
        /// <summary>
        /// 제안유형(구분자 ',')
        /// </summary>
        public string ProposalType { get; set; } = null!;
        /// <summary>
        /// 회원아이디
        /// </summary>
        public string Uid { get; set; } = null!;
        /// <summary>
        /// 멤버아이디
        /// </summary>
        public int Mid { get; set; }
        /// <summary>
        /// 내용
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// 삭제유무
        /// </summary>
        public string DelYn { get; set; } = null!;
        /// <summary>
        /// 등록날짜
        /// </summary>
        public int RegDt { get; set; }
        /// <summary>
        /// 확인 관리자 아이디
        /// </summary>
        public string? ApprAid { get; set; }
        /// <summary>
        /// 관리자 확인시간
        /// </summary>
        public int ApprDt { get; set; }
    }
}
