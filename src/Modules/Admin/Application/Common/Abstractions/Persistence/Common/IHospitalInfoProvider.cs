using Hello100Admin.Modules.Admin.Application.Common.ReadModels;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common
{
    public interface IHospitalInfoProvider
    {
        /// <summary>
        /// 요양기관번호로 요양기관 프로필 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetHospitalInfoReadModel?> GetHospitalInfoByHospNoAsync(string hospNo, CancellationToken token);

        /// <summary>
        /// 요양기관 키로 요양기관 프로필 조회
        /// </summary>
        /// <param name="hospKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetHospitalInfoReadModel?> GetHospitalInfoByHospKeyAsync(string hospKey, CancellationToken token);
    }
}
