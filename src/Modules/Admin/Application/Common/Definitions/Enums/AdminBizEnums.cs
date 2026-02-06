namespace Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums
{
    /// <summary>
    /// Admin Module 전체에서 사용할 비즈니스 Enums (ErrorCode 제외)
    /// </summary>
    public enum ImageUploadType
    {
        None = 0,
        HO = 1,
        PO = 2, // 팝업 광고 이미지
        BA = 3, // 배너 광고 이미지
        HospInfo = 4,  // 병원정보
        CK = 5,        // 에디터 이미지
        PU = 6,        // 푸시 이미지
        UNTACT = 7,     // 비대면
        PHARM = 8     // 약국

    }

    public enum Hello100RoleType
    {
        /// <summary>
        /// QR 접수    1
        /// </summary>
        QR = 1 << 0,
        /// <summary>
        /// 당일 접수  2
        /// </summary>
        Recept = 1 << 1,
        /// <summary>
        /// 진료 예약   4
        /// </summary>
        Rsrv = 1 << 2,
        /// <summary>
        /// QR 접수 불가 8
        /// </summary>
        NoQR = 1 << 3,
        /// <summary>
        /// 당일 접수 마감 체크  16
        /// </summary>
        NoRecept = 1 << 4,
        /// <summary>
        /// 비대면 접수 32
        /// </summary>
        UntactRecept = 1 << 5
    }
}
