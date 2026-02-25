namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    /// <summary>
    /// 키워드 상세 정보
    /// </summary>
    public class TbKeywordDetailEntity
    {
        /// <summary>
        /// 공통시퀀스
        /// </summary>
        public int DetailSeq { get; set; }

        public int MasterSeq { get; set; }

        public string DetailName { get; set; } = null!;

        public int SortNo { get; set; }

        public int SearchCnt { get; set; }
    }
}
