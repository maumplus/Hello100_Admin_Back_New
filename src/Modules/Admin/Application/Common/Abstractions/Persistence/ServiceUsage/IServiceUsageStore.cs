using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportUntactMedicalHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchUntactMedicalHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.ExportUntactMedicalHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.GetUntactMedicalPaymentDetail;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.ReadModels.SearchUntactMedicalHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results;
using Hello100Admin.Modules.Admin.Domain.Entities;

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

        /// <summary>
        /// 알림톡 신청 정보 존재 여부 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hospNo"></param>
        /// <param name="hospKey"></param>
        /// <param name="tmpType"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<TbKakaoMsgJoinEntity?> FindAlimtalkServiceApplicationAsync(DbSession db, string hospNo, string hospKey, string tmpType, CancellationToken ct);

        /// <summary>
        /// 서비스 단위 접수현황 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="searchType"></param>
        /// <param name="searchChartType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="qrCheckYn"></param>
        /// <param name="todayRegistrationYn"></param>
        /// <param name="appointmentYn"></param>
        /// <param name="telemedicineYn"></param>
        /// <param name="excludeTestHospitalsYn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<List<GetHospitalServiceUsageStatusResultItemByServiceUnit>> GetServiceUnitReceptionStatusAsync(
            DbSession db, string? fromDate, string? toDate, string? searchChartType, int searchType, string? searchKeyword, string qrCheckYn,
            string todayRegistrationYn, string appointmentYn, string telemedicineYn, string excludeTestHospitalsYn, CancellationToken ct);

        /// <summary>
        /// 병원 단위 접수현황 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="searchChartType"></param>
        /// <param name="searchType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="qrCheckYn"></param>
        /// <param name="todayRegistrationYn"></param>
        /// <param name="appointmentYn"></param>
        /// <param name="telemedicineYn"></param>
        /// <param name="excludeTestHospitalsYn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ListResult<GetHospitalServiceUsageStatusResultItemByHospitalUnit>> GetHospitalUnitReceptionStatusAsync(
            DbSession db, int pageNo, int pageSize, string? fromDate, string? toDate, string? searchChartType, int searchType, string? searchKeyword, string qrCheckYn,
            string todayRegistrationYn, string appointmentYn, string telemedicineYn, string excludeTestHospitalsYn, CancellationToken ct);

        /// <summary>
        /// 병원 단위 접수현황 엑셀 출력
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="searchChartType"></param>
        /// <param name="searchType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="qrCheckYn"></param>
        /// <param name="todayRegistrationYn"></param>
        /// <param name="appointmentYn"></param>
        /// <param name="telemedicineYn"></param>
        /// <param name="excludeTestHospitalsYn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<List<GetHospitalServiceUsageStatusResultItemByHospitalUnit>> ExportHospitalUnitReceptionStatusExcelAsync(
            DbSession db, string? fromDate, string? toDate, string? searchChartType, int searchType, string? searchKeyword, string qrCheckYn,
            string todayRegistrationYn, string appointmentYn, string telemedicineYn, string excludeTestHospitalsYn, CancellationToken ct);

        /// <summary>
        /// 헬로100 접수 현황 엑셀 출력
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<ExportHello100ReceptionStatusExcelResult> ExportHello100ReceptionStatusExcelAsync(
            DbSession db, string fromDate, string toDate, CancellationToken ct);

        /// <summary>
        /// 비대면 진료 현황 조회
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="searchDateType"></param>
        /// <param name="searchType"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="searchStateTypes"></param>
        /// <param name="searchPaymentTypes"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<GetUntactMedicalUsageStatusResult> GetUntactMedicalUsageStatusAsync(
            DbSession db, int pageNo, int pageSize, string fromDate, string toDate, int searchDateType, int searchType, string? searchKeyword,
            List<string> searchStateTypes, List<string> searchPaymentTypes, CancellationToken ct);

        public Task<List<ExportUntactMedicalUsageStatusExcelResult>> ExportUntactMedicalUsageStatusExcelAsync(
            DbSession db, string fromDate, string toDate, int searchDateType, int searchType, string? searchKeyword,
            List<string> searchStateTypes, List<string> searchPaymentTypes, CancellationToken ct);
    }
}
