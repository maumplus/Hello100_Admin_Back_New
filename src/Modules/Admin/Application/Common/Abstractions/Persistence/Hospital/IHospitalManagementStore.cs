using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.ReadModels;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital
{
    public interface IHospitalManagementStore
    {
        Task<(List<GetHospitalResult>, int)> GetHospitalListAsync(string chartType, HospitalListSearchType searchType, string keyword, int pageNo, int pageSize, CancellationToken cancellationToken = default);
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
        Task<List<GetDoctorListModel>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default);
        /// <summary>
        /// 전체 진료과목 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="clsCd"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ListResult<GetMedicalDepartmentsResult>> GetMedicalDepartmentsAsync(DbSession db, string clsCd, CancellationToken cancellationToken = default);
        /// <summary>
        /// 증상/검진 키워드 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keyword"></param>
        /// <param name="masterSeq"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<List<GetClinicalKeywordsResult>> GetClinicalKeywordsAsync(DbSession db, string? keyword, string? masterSeq, CancellationToken ct = default);
    }
}
