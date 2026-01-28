using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportUntactMedicalHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchUntactMedicalHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportUntactMedicalHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.GetUntactMedicalPaymentDetail;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchUntactMedicalHistories;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage
{
    public interface IServiceUsageStore
    {
        /// <summary>
        /// 비대면 진료 내역 리스트 조회
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<SearchUntactMedicalHistoriesReadModel> SearchUntactMedicalHistoriesAsync(SearchUntactMedicalHistoriesQuery req, CancellationToken token);

        /// <summary>
        /// 비대면 진료 결제 내역 상세 조회
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetUntactMedicalPaymentDetailReadModel?> GetUntactMedicalPaymentDetailAsync(string paymentId, CancellationToken token);

        /// <summary>
        /// 비대면 진료 내역 엑셀 Export
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<GetUntactMedicalHistoryForExportReadModel>> GetUntactMedicalHistoryForExportAsync(ExportUntactMedicalHistoriesExcelQuery req, CancellationToken token);

        /// <summary>
        /// 진단검사 결과 알림톡 발송 이력 조회
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<SearchExaminationResultAlimtalkHistoriesReadModel> SearchExaminationResultAlimtalkHistoriesAsync(SearchExaminationResultAlimtalkHistoriesQuery req, CancellationToken token);

        /// <summary>
        /// 진단검사 결과 알림톡 발송 이력 엑셀 Export
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<List<GetExaminationResultAlimtalkHistoryForExportReadModel>> GetExaminationResultAlimtalkHistoryForExportAsync(ExportExaminationResultAlimtalkHistoriesExcelQuery req, CancellationToken token);
    }
}
