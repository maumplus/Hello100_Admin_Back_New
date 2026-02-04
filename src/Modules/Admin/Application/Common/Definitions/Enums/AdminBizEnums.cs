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
}
