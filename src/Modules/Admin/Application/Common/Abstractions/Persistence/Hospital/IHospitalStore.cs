using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital
{
    public interface IHospitalStore
    {
        Task<(List<GetHospitalModel>, int)> GetHospitalList(string chartType, HospitalListSearchType searchType, string keyword, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<GetHospitalModel?> GetHospital(string hospNo, CancellationToken cancellationToken = default);
        Task<List<GetHospMedicalTimeModel>> GetHospMedicalTimeList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetHospKeywordModel>> GetHospKeywordList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetHospitalMedicalModel>> GetHospitalMedicalList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetImageModel>> GetImageList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetHospMedicalTimeNewModel>> GetHospMedicalTimeNewList(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetKeywordMasterModel>> GetKeywordMasterList(string hospKey, CancellationToken cancellationToken = default);
        Task<GetHospSettingModel?> GetHospSetting(string hospKey, CancellationToken cancellationToken = default);
        Task<List<GetDoctorListModel>> GetDoctorList(string hospNo, CancellationToken cancellationToken = default);
    }
}
