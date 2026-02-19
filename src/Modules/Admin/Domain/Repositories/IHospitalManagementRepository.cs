using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Dapper;
using Hello100Admin.Modules.Admin.Domain.Entities;
using System.Data;

namespace Hello100Admin.Modules.Admin.Domain.Repositories
{
    public interface IHospitalManagementRepository
    {
        /// <summary>
        /// 병원 정보 등록/수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="aId"></param>
        /// <param name="hospKey"></param>
        /// <param name="apprType"></param>
        /// <param name="reqJson"></param>
        /// <param name="imageUrls"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpsertHospitalAsync(
            DbSession db, string aId, string hospKey, string apprType, string reqJson, List<string> imageUrls, CancellationToken ct);


        /// <summary>
        /// 병원 정보(승인) 등록/수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="aId"></param>
        /// <param name="apprId"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospKey"></param>
        /// <param name="description"></param>
        /// <param name="businessNo"></param>
        /// <param name="businessLevel"></param>
        /// <param name="mainMdCd"></param>
        /// <param name="clinicTimesEntity"></param>
        /// <param name="clinicTimesNewEntity"></param>
        /// <param name="deptCodesEntity"></param>
        /// <param name="keywordsEntity"></param>
        /// <param name="imagesEntity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpsertAdmHospitalAsync
            (DbSession db, string aId, int apprId, string hospNo, string hospKey, string? description, string? businessNo, string? businessLevel, string? mainMdCd,
            List<TbEghisHospMedicalTimeEntity> clinicTimesEntity, List<TbEghisHospMedicalTimeNewEntity> clinicTimesNewEntity,
            List<TbHospitalMedicalInfoEntity> deptCodesEntity, List<TbEghisHospKeywordInfoEntity> keywordsEntity, List<TbImageInfoEntity> imagesEntity, CancellationToken ct);

        /// <summary>
        /// Hello100 설정 등록/수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="settingEntity"></param>
        /// <param name="noticeEntity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpsertHello100SettingAsync(DbSession db, string hospNo, TbEghisHospSettingsInfoEntity settingEntity, TbNoticeEntity noticeEntity, CancellationToken ct);

        /// <summary>
        /// 디바이스(헬로데스크, 키오스크) 설정 등록/수정
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospNm"></param>
        /// <param name="emplNo"></param>
        /// <param name="deviceNm"></param>
        /// <param name="deviceType"></param>
        /// <param name="hospKey"></param>
        /// <param name="infoTxt"></param>
        /// <param name="useYn"></param>
        /// <param name="setJson"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<int> UpsertDeviceSettingAsync(
            DbSession db, string hospNo, string hospNm, string emplNo, string deviceNm, int deviceType, string hospKey, string infoTxt, string useYn, string? setJson, CancellationToken ct);
        public Task<int> UpdateDoctorInfoAsync(DbSession db, EghisDoctInfoEntity eghisDoctInfo, CancellationToken ct);
        public Task<int> UpdateDoctorInfoFileAsync(DbSession db, TbEghisDoctInfoFileEntity eghisDoctInfo, CancellationToken ct);
        public Task<int> UpdateDoctorUntactAsync(DbSession db, TbEghisDoctUntactEntity eghisDoctUntact, CancellationToken ct);
        public Task<int> RemoveEghisDoctRsrvAsync(DbSession db, int ridx, string receptType, CancellationToken ct);
        public Task<int> RemoveEghisDoctRsrvAsync(DbSession db, EghisDoctRsrvInfoEntity eghisDoctRsrvInfoEntity, string receptType, CancellationToken ct);
        public Task<int> UpdateDoctorInfoScheduleAsync(DbSession db, List<EghisDoctInfoEntity> eghisDoctInfoList, CancellationToken ct);
        public Task<int> UpdateDoctorInfoUntactScheduleAsync(DbSession db, List<EghisDoctInfoEntity> eghisDoctInfoList, CancellationToken ct);
        public Task<int> InsertEghisDoctRsrvAsync(DbSession db, EghisDoctRsrvInfoEntity eghisDoctRsrvInfoEntity, CancellationToken ct);
        public Task<int> InsertEghisDoctUntactRsrvAsync(DbSession db, EghisDoctRsrvInfoEntity eghisDoctRsrvInfoEntity, CancellationToken ct);
        public Task<int> InsertEghisDoctRsrvDetailAsync(DbSession db, List<EghisDoctRsrvDetailInfoEntity> eghisDoctRsrvDetailInfoList, CancellationToken ct);
        public Task<int> RemoveEghisDoctInfoMdAsync(DbSession db, EghisDoctInfoMdEntity eghisDoctInfoMdEntity, CancellationToken ct);
        public Task<int> InsertEghisDoctInfoMdAsync(DbSession db, EghisDoctInfoMdEntity eghisDoctInfoMdEntity, CancellationToken ct);
        public Task<int> InsertEghisDoctUntactJoinAsync(DbSession db, TbEghisDoctUntactJoinEntity tbEghisDoctUntactJoinEntity, CancellationToken ct);
    }
}
