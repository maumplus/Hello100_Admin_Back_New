using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Admin.Application.Features.Account.ReadModels;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account
{
    public interface IAccountStore
    {
        Task<(List<GetHospitalModel>, int)> GetHospitalList(AccountHospitalListSearchType searchType, string keyword, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<List<TbCommonEntity>> GetCommonList(string clsCd, CancellationToken cancellationToken = default);
    }
}
