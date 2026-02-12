namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results
{
    public sealed class GetHospitalDetailResult
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = default!;
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
        /// 대표번호
        /// </summary>
        public string Tel { get; set; } = default!;
        /// <summary>
        /// 홈페이지
        /// </summary>
        public string? Site { get; set; }
        /// <summary>
        /// 위경도 x좌표
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 위경도 y좌표
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 차트타입 [E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string ChartType { get; set; } = default!;
        /// <summary>
        /// 테스트병원여부
        /// 0:일반병원
        /// 1:테스트병원
        /// </summary>
        public int IsTest { get; set; }
        /// <summary>
        /// 진료과 코드들(콤마 단위 분리)
        /// </summary>
        public string? DeptCd { get; set; }
        /// <summary>
        /// 진료과 코드명들 (콤마 단위 분리)
        /// </summary>
        public string? DeptName { get; set; }
        /// <summary>
        /// 진료과 코드정보 리스트
        /// </summary>
        public List<GetHospitalDetailResultDeptCode>? DeptCodes { get; set; }
    }

    /// <summary>
    /// UpsertHospitalRequest.cs MedicalInfoBase
    /// </summary>
    public class GetHospitalDetailResultDeptCode
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; set; } = default!;
        /// <summary>
        /// 진료과 코드
        /// </summary>
        public string MdCd { get; set; } = default!;
        /// <summary>
        /// 진료과 코드명
        /// </summary>
        public string MdNm { get; set; } = default!;
    }
}
