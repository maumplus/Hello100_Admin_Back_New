namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public sealed class GetHospitalResult
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = default!;
        /// <summary>
        /// 요양기관 번호
        /// </summary>
        public string HospNo { get; set; } = default!;
        /// <summary>
        /// 사업자등록번호
        /// </summary>
        public string? BusinessNo { get; set; }
        /// <summary>
        /// 사업자구분
        /// </summary>
        public string? BusinessLevel { get; set; }
        /// <summary>
        /// 병원명
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// 종별코드
        /// </summary>
        public string HospClsCd { get; set; } = default!;
        /// <summary>
        /// 주소
        /// </summary>
        public string Addr { get; set; } = default!;
        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostCd { get; set; } = default!;
        /// <summary>
        /// 전화번호
        /// </summary>
        public string Tel { get; set; } = default!;
        /// <summary>
        /// 사이트
        /// </summary>
        public string? Site { get; set; }
        /// <summary>
        /// 위도
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 경도
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 폐업여부
        /// </summary>
        public string ClosingYn { get; set; } = default!;
        /// <summary>
        /// 삭제여부
        /// </summary>
        public string DelYn { get; set; } = default!;
        /// <summary>
        /// 상세정보
        /// </summary>
        public string? Descrption { get; set; }
        /// <summary>
        /// 등록일시
        /// </summary>
        public string RegDt { get; set; } = default!;
        /// <summary>
        /// 차트타입 [E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string ChartType { get; set; } = default!;
        /// <summary>
        /// 테스트병원여부 [0: 일반, 1: 테스트]
        /// </summary>
        public int IsTest { get; set; }
        /// <summary>
        /// 진료과 코드
        /// </summary>
        public string? MdCd { get; set; }
        /// <summary>
        /// 대표 진료과 코드
        /// </summary>
        public string? MainMdCd { get; set; }
        /// <summary>
        /// 키오스크 수
        /// </summary>
        public int KioskCnt { get; set; }
        /// <summary>
        /// 태블릿 수
        /// </summary>
        public int TabletCnt { get; set; }
        /// <summary>
        /// 승인요청여부(?)
        /// </summary>
        public int RequestApprYn { get; set; }
        /// <summary>
        /// 병원운영시간
        /// </summary>
        public List<MedicalTimeResultItem>? ClinicTimes { get; set; }
        /// <summary>
        /// 진료과목
        /// </summary>
        public List<MedicalInfoResultItem>? DeptCodes { get; set; }
        /// <summary>
        /// 증상/검진 키워드
        /// </summary>
        public List<HashTagInfoResultItem>? Keywords { get; set; }
        /// <summary>
        /// 이미지정보
        /// </summary>
        public List<ImageInfoResultItem>? Images { get; set; }
        /// <summary>
        /// 진료시간
        /// </summary>
        public List<MedicalTimeNewResultItem>? ClinicTimesNew { get; set; }
        /// <summary>
        /// 증상/검진 키워드 전체 (키워드 마스터정보)
        /// </summary>
        public List<KeywordMasterResultItem>? KeywordMasters { get; set; }
    }

    public class MedicalTimeResultItem
    {
        public long MtId { get; set; }
        public string HospKey { get; set; }
        public string MtNm { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
    }

    public class MedicalInfoResultItem
    {
        public string MdCd { get; set; }
        public string HospKey { get; set; }
        public string MdNm { get; set; }
        public string RegDt { get; set; }
    }

    public class HashTagInfoResultItem
    {
        public int TagId { get; set; }
        public string Kid { get; set; }
        public string HospKey { get; set; }
        public string TagNm { get; set; }
        public string Keyword { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
        public int MasterSeq { get; set; }
        public int DetailSeq { get; set; }
    }

    public class ImageInfoResultItem
    {
        public int ImgId { get; set; }
        public string ImgKey { get; set; }
        public string Url { get; set; }
        public string DelYn { get; set; }
        public string RegDt { get; set; }
    }

    public class MedicalTimeNewResultItem
    {
        public string HospKey { get; set; }
        public string HospNo { get; set; }
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

    public class KeywordMasterResultItem
    {
        public string Keyword { get; set; }
        public int MasterSeq { get; set; }
        public string MasterName { get; set; }
        public string DetailUseYn { get; set; }
        public string ShowYn { get; set; }
        public int SortNo { get; set; }
        public int SearchCnt { get; set; }
        public int DetailCnt { get; set; }
        public string RegDt { get; set; }
        public int DetailSeq { get; set; }
    }
}
