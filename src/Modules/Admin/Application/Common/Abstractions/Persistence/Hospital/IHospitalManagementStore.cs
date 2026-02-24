using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital
{
    public interface IHospitalManagementStore
    {
        /// <summary>
        /// 병원정보관리 > 병원 목록 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchChartType"></param>
        /// <param name="searchType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListResult<GetHospitalsUsingHello100ServiceResult>> GetHospitalsUsingHello100ServiceAsync(DbSession db, int pageNo, int pageSize, string? searchChartType, int searchType, string? searchKeyword, CancellationToken cancellationToken);
        /// <summary>
        /// 병원정보관리 > 병원 상세정보 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetHospitalResult?> GetHospitalAsync(string hospNo, CancellationToken cancellationToken = default);
        /// <summary>
        /// 병원 운영시간 목록 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MedicalTimeResultItem>> GetHospMedicalTimeListAsync(string hospKey, CancellationToken cancellationToken = default);
        /// <summary>
        /// 병원 증상/검진 키워드 목록 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<HashTagInfoResultItem>> GetHospKeywordListAsync(string hospKey, CancellationToken cancellationToken = default);
        /// <summary>
        /// 병원 진료과목 목록 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MedicalInfoResultItem>> GetHospitalMedicalListAsync(string hospKey, CancellationToken cancellationToken = default);
        /// <summary>
        /// 병원 이미지 정보 목록 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<ImageInfoResultItem>> GetImageListAsync(string hospKey, CancellationToken cancellationToken = default);
        /// <summary>
        /// 병원 진료시간 목록 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MedicalTimeNewResultItem>> GetHospMedicalTimeNewListAsync(string hospKey, CancellationToken cancellationToken = default);
        /// <summary>
        /// 병원 증상/검진 키워드 전체 (키워드 마스터정보) 목록 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<KeywordMasterResultItem>> GetKeywordMasterListAsync(string hospKey, CancellationToken cancellationToken = default);
        /// <summary>
        /// Hello100 설정 정보 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospKey"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<GetHello100SettingResult?> GetHello100SettingAsync(DbSession db, string hospKey, CancellationToken ct = default);

        /// <summary>
        /// 헬로데스크 설정 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospKey"></param>
        /// <param name="emplNo"></param>
        /// <param name="deviceType"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<GetDeviceSettingResult<TabletRo>> GetHelloDeskSettingAsync(
            DbSession db, string hospNo, string hospKey, string? emplNo, int deviceType, CancellationToken ct = default);

        /// <summary>
        /// 키오스크 설정 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospKey"></param>
        /// <param name="emplNo"></param>
        /// <param name="deviceType"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<GetDeviceSettingResult<KioskRo>> GetKioskSettingAsync(
            DbSession db, string hospNo, string hospKey, string? emplNo, int deviceType, CancellationToken ct = default);

        /// <summary>
        /// 의사 목록 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ListResult<DoctorBaseRo>> GetDoctorsAsync(
            DbSession db, string hospNo, int pageNo, int pageSize, CancellationToken ct = default);
        public Task<List<GetDoctorListResult>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default);
        public Task<TbEghisDoctUntactEntity?> GetDoctorUntactInfo(string hospNo, string emplNo, CancellationToken cancellationToken = default);
        public Task<List<GetDoctorScheduleResult>> GetDoctorList(string hospNo, string emplNo, CancellationToken cancellationToken = default);
        public Task<EghisDoctRsrvInfoEntity?> GetEghisDoctRsrvInfo(string hospNo, string emplNo, int weekNum, string clinicYmd, CancellationToken cancellationToken = default);
        public Task<List<EghisDoctRsrvDetailInfoEntity>> GetEghisDoctRsrvDetailList(int ridx, string receptType, CancellationToken cancellationToken = default);
        public Task<List<EghisRsrvInfoEntity>> GetEghisRsrvList(string hospNo, string emplNo, int weekNum, CancellationToken cancellationToken = default);
        public Task<List<EghisRsrvInfoEntity>> GetEghisRsrvList(string hospNo, string emplNo, string clinicYmd, CancellationToken cancellationToken = default);
        public Task<List<EghisRsrvInfoEntity>> GetEghisUntactRsrvList(string hospNo, string emplNo, int weekNum, CancellationToken cancellationToken = default);
        public Task<List<EghisDoctInfoMdEntity>> GetEghisDoctInfoMd(string hospNo, string emplNo, CancellationToken cancellationToken = default);
        /// <summary>
        /// 의사 정보 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="emplNo"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<EghisDoctInfoEntity?> GetDoctorInfoAsync(DbSession db, string hospNo, string emplNo, CancellationToken ct);
    }
}
