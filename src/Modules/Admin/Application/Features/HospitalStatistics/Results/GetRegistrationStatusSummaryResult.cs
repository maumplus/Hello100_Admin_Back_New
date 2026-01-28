namespace Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results
{
    public sealed class GetRegistrationStatusSummaryResult
    {
        /// <summary>
        /// 월명
        /// </summary>
        public string MonthNm { get; set; } = default!;

        /// <summary>
        /// 접수 현황
        /// </summary>
        public int Recept { get; set; }

        /// <summary>
        /// 취소 현황
        /// </summary>
        public int Cancel { get; set; }

        //public string HospNo { get; set; } = default!;
        //public StatPtntType CreatePtntType { get; set; } = StatPtntType.Recept;
    }
}
