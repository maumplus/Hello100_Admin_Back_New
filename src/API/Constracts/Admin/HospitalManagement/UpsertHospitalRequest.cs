namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public record UpsertHospitalRequest
    {
        /// <summary>
        /// 사업자등록번호
        /// </summary>
        public string? BusinessNo { get; init; }
        /// <summary>
        /// 사업자 구분
        /// </summary>
        public string? BusinessLevel { get; init; }
        /// <summary>
        /// 상세정보
        /// 기존에 Descrption 이라고 돼있었음. 오타아님.
        /// tb_eghis_hosp_approval_info.data colun에 json으로 저장되는 데이터라 임의로 변경 하지 않음.
        /// </summary>
        public string? Descrption { get; init; }
        /// <summary>
        /// 대표 진료과
        /// </summary>
        public string? MainMdCd { get; init; }
        /// <summary>
        /// 병원운영시간
        /// </summary>
        public List<MedicalTimeBase>? ClinicTimes { get; init; }
        /// <summary>
        /// 진료시간
        /// </summary>
        public List<MedicalTimeBaseNew>? ClinicTimesNew { get; init; }
        /// <summary>
        /// 진료과목
        /// </summary>
        public List<MedicalInfoBase>? DeptCodes { get; init; }
        /// <summary>
        /// 증상/검진 키워드
        /// </summary>
        public List<HashTagInfoBase>? Keywords { get; init; }
        /// <summary>
        /// 기존 이미지정보
        /// </summary>
        public List<ImageInfoBase>? ImgFiles { get; init; }
        /// <summary>
        /// 신규 이미지 목록
        /// </summary>
        public List<IFormFile>? NewImages { get; init; }
    }

    public class MedicalTimeBase
    {
        public int MtId { get; set; }
        //public string HospKey { get; set; }
        public string MtNm { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
    }

    public class MedicalTimeBaseNew
    {
        //public string HospKey { get; set; }
        //public string HospNo { get; set; }
        public int WeekNum { get; set; }
        public string WeekNumNm { get; set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }
        public string EndHour { get; set; }
        public string EndMinute { get; set; }
        public string BreakStartHour { get; set; }
        public string BreakStartMinute { get; set; }
        public string BreakEndHour { get; set; }
        public string BreakEndMinute { get; set; }
        public string UseYn { get; set; }
    }

    public class MedicalInfoBase
    {
        public string MdCd { get; set; }
        //public string HospKey { get; set; }
        public string MdNm { get; set; }
        public string RegDt { get; set; }
    }

    public class HashTagInfoBase
    {
        public int TagId { get; set; }
        public string Kid { get; set; }
        //public string HospKey { get; set; }
        public string TagNm { get; set; }
        public string Keyword { get; set; }
        public string DelYn { get; set; }
        //public string RegDt { get; set; }
        public int MasterSeq { get; set; }
        public int DetailSeq { get; set; }
    }

    public class ImageInfoBase
    {
        public int ImgId { get; set; }
        public string ImgKey { get; set; }
        public string Url { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
    }
}
