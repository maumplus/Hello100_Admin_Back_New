using System.ComponentModel;

namespace Hello100Admin.Modules.Admin.Application.Common.Errors
{
    /// <summary>
    /// 범위 : 4001 ~ 8000 
    /// 추후 에러코드 관련 정의 필요 [Module당 Number or 별도 Code 세팅]
    /// </summary>
    public enum AdminErrorCode
    {
        [Description("비밀번호 변경에 실패하였습니다. 다시 시도해주세요.")]
        PasswordChangeFailed = 4001,
        [Description("비대면 진료 결제 내역을 찾을 수 없습니다.")]
        NotFoundUntactMedicalPayment = 4002,
        [Description("카카오 비즈 데이터 요청 간 에러가 발생하였습니다. 다시 시도해주세요.")]
        KakaoBizDataRequestFailed = 4003,
        [Description("병원 정보를 찾지 못했습니다. 다시 로그인해주세요.")]
        NotFoundCurrentHospital = 4004,
        [Description("비즈사이트 카카오 알림톡 서비스 신청에 실패하였습니다. 다시 시도해주세요.")]
        RequestKakaoAlimTalkServiceFailed = 4005,
        [Description("내원목적 순번 및 노출 여부 수정에 실패하였습니다. 다시 시도해주세요.")]
        VisitPurposeBulkUpdateFailed = 4006,
        [Description("승인 요청에 실패하였습니다. 다시 시도해주세요.")]
        VisitPurposeCreateFailed = 4007,
        [Description("내원목적은 한 개 이상 노출이 필요합니다.")]
        VisitPurposeExposureRequired = 4008,
        [Description("내원목적 수정에 실패하였습니다.")]
        VisitPurposeUpdateFailed = 4009,
        [Description("내원목적 삭제에 실패하였습니다. 이미 삭제된 데이터이거나, 잘못된 데이터일 수 있습니다.")]
        VisitPurposeDeleteFailed = 4010,
        [Description("제증명문서 순번 및 노출 여부 수정에 실패하였습니다.")]
        CertificateBulkUpdateFailed = 4011,
        [Description("해당 회원 정보를 찾지 못하였습니다. 확인 후 다시 시도해주세요.")]
        NotFoundUserInfo = 4012,
        [Description("회원 권한 수정에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        UpdateUserRoleFailed = 4013,
        [Description("회원 프로필 삭제에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        DeleteUserFamilyFailed = 4014,
        [Description("회원 삭제에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        DeleteUserFailed = 4015,
        [Description("SFTP 서버 업로드에 실패하였습니다. 다시 시도해주세요.")]
        SftpUploadFailed = 4016,
        [Description("허용되지 않은 확장자입니다. 확인 후 다시 시도해주세요.")]
        NotAllowedExtensions = 4017,
        [Description("디렉토리 경로가 존재하지 않습니다. 확인 후 다시 시도해주세요.")]
        NotFoundDirectory = 4018,
        [Description("파일명이 존재하지 않습니다. 확인 후 다시 시도해주세요.")]
        NotFoundFileName = 4019,
        [Description("서버 디렉토리 생성에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        CreateDirectoryFailed = 4020,
        [Description("파일 스트림 읽기에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        NotFoundFileStream = 4021,
        [Description("광고 등록에 실패하였습니다. 다시 시도해주세요.")]
        CreateAdvertisementFailed = 4022,
        [Description("팝업 광고 갱신에 실패하였습니다. 다시 시도해주세요.")]
        UpdatePopupAdvertisementFailed = 4023,
        [Description("광고가 존재하지 않습니다. 확인 후 다시 시도해주세요.")]
        AdvertisementNotFound = 4024,
        [Description("광고 삭제에 실패하였습니다. 다시 시도해주세요.")]
        DeleteAdvertisementFailed = 4025,
        [Description("SFTP 서버 연결에 실패하였습니다. 다시 시도해주세요.")]
        SftpConnectionFailed = 4026,
        [Description("병원정보 저장 또는 수정에 실패하였습니다. 다시 시도해주세요.")]
        UpsertHospitalFailed = 4027,
        [Description("비즈 사이트 데이터 요청 간 에러가 발생하였습니다. 다시 시도해주세요.")]
        BizSiteDataRequestFailed = 4028,
        [Description("헬로100 설정 저장 또는 수정에 실패하였습니다. 다시 시도해주세요.")]
        UpsertHello100SettingFailed = 4029,
        [Description("입력하신 요양기관은 이미 모바일 사용과 연결되어 있습니다. 관리자에게 문의하세요.")]
        ExistsHospMappingInfo = 4030,
        [Description("요양기관번호 설정에 실패하였습니다. 관리자에게 문의하세요.")]
        HospitalInfoSaveFaild = 4031,
        [Description("기기 설정 저장 또는 수정에 실패하였습니다. 다시 시도해주세요.")]
        UpsertDeviceSettingFailed = 4032,
        [Description("저장 또는 수정에 필요한 데이터가 없습니다. 다시 시도해주세요.")]
        NoDataToUpsert = 4033,
        [Description("등록된 태블릿이 없습니다. 확인 후 다시 시도해주세요.")]
        TabletNotRegistered = 4034,
        [Description("등록된 키오스크가 없습니다. 확인 후 다시 시도해주세요.")]
        KioskNotRegistered = 4035,
        [Description("이지스 배너 광고 순번 및 노출 여부 수정에 실패하였습니다. 다시 시도해주세요.")]
        EghisBannerBulkUpdateFailed = 4036,
        [Description("이지스 배너 광고 갱신에 실패하였습니다. 다시 시도해주세요.")]
        UpdateEghisBannerAdvertisementFailed = 4037,
        [Description("공지사항 등록에 실패하였습니다. 다시 시도해주세요.")]
        CreateNoticeFailed = 4038,
        [Description("상세 조회할 공지사항 데이터가 없습니다. 확인 후 다시 시도해주세요.")]
        NoticeNotFound = 4039,
        [Description("공지사항 갱신에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        UpdateNoticeFailed = 4040,
        [Description("공지사항 삭제에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        DeleteNoticeFailed = 4041,
        [Description("병원 정보 조회에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        NotFoundHospital = 4042,
        [Description("신규 병원 생성에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        CreateHospitalFailed = 4043,
        [Description("병원 삭제에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        DeleteHospitalFailed = 4044,
        [Description("관리자확인 갱신에 실패하였습니다. 확인 후 다시 시도해주세요.")]
        UpdateRequestBug = 4045,
        [Description("해당 의사 정보를 찾지 못하였습니다. 확인 후 다시 시도해주세요.")]
        NotFoundDoctorInfo = 4046,
        [Description("내원목적 정보를 찾지 못하였습니다. 확인 후 다시 시도해주세요.")]
        NotFoundVisitPurpose = 4047,
    }
}
