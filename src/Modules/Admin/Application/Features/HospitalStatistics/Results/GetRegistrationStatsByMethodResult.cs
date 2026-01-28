namespace Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results
{
    public sealed class GetRegistrationStatsByMethodResult
    {
        //public string HospNo { get; set; } = default!;
        /// <summary>
        /// 월명
        /// </summary>
        public string MonthNm { get; set; } = default!;

        /// <summary>
        /// QR접수
        /// </summary>
        public int QrRecept { get; set; }

        /// <summary>
        /// QR취소
        /// </summary>
        public int QrCancel { get; set; }

        /// <summary>
        /// 당일접수
        /// </summary>
        public int Recept { get; set; }

        /// <summary>
        /// 당일취소
        /// </summary>
        public int ReceptCancel { get; set; }

        /// <summary>
        /// 예약접수
        /// </summary>
        public int Rsrv { get; set; }

        /// <summary>
        /// 예약취소
        /// </summary>
        public int RsrvCancel { get; set; }

        /// <summary>
        /// 비대면접수
        /// </summary>
        public int NonContact { get; set; }

        /// <summary>
        /// 비대면취소
        /// </summary>
        public int NonContactCancel { get; set; }
    }
}
