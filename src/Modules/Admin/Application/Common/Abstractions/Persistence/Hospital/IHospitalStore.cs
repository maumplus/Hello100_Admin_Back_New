using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.ReadModels;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital
{
    public interface IHospitalStore
    {
        Task<(List<GetHospitalResult>, int)> GetHospitalList(string chartType, HospitalListSearchType searchType, string keyword, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<GetHospitalResult?> GetHospital(string hospNo, CancellationToken cancellationToken = default);
        Task<List<MedicalTimeResultItem>> GetHospMedicalTimeList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<HashTagInfoResultItem>> GetHospKeywordList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<MedicalInfoResultItem>> GetHospitalMedicalList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<ImageInfoResultItem>> GetImageList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<MedicalTimeNewResultItem>> GetHospMedicalTimeNewList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<KeywordMasterResultItem>> GetKeywordMasterList(string hospKey, CancellationToken cancellationToken = default);
        Task<GetHospSettingModel?> GetHospSetting(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetDoctorListModel>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default);
    }
}
