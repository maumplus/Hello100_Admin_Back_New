namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbKakaoMsgJoinEntity
    {
        public string HospNo { get; set; } = default!;
        public string HospKey { get; set; } = default!;
        public string DoctNm { get; set; } = default!;
        public string DoctTel { get; set; } = default!;
        public string TmpType { get; set; } = "";
        public int RegDt { get; set; }
    }
}
