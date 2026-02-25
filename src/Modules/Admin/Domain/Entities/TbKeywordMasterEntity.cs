namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbKeywordMasterEntity
    {
        /// <summary>
        /// 공통시퀀스
        /// </summary>
        public int MasterSeq { get; set; }

        public string MasterName { get; set; } = null!;

        public string DetailUseYn { get; set; } = null!;

        public string ShowYn { get; set; } = null!;

        public int SortNo { get; set; }

        public int SearchCnt { get; set; }

        public int? ModDt { get; set; }

        public int RegDt { get; set; }
    }
}
