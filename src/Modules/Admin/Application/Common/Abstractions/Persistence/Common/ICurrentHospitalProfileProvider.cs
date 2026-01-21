using Hello100Admin.Modules.Admin.Application.Common.ReadModels;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common
{
    public interface ICurrentHospitalProfileProvider
    {
        /// <summary>
        /// 현재 로그인 사용자 요양기관 프로필 조회
        /// </summary>
        /// <param name="hospNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetCurrentHospitalProfileReadModel> GetCurrentHospitalProfileByHospNoAsync(string hospNo, CancellationToken token);
    }
}
