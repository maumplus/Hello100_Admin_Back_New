using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Queries
{
    public record SearchHospitalUsersQuery : IQuery<Result<ListResult<SearchHospitalUsersResult>>>
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNo { get; init; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; init; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; init; }

        /// <summary>
        /// 검색 키워드 조회 타입 [Name: 1, Email: 2, Phone: 3]
        /// </summary>
        public int KeywordSearchType { get; init; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }

    public class SearchHospitalUsersQueryValidator : AbstractValidator<SearchHospitalUsersQuery>
    {
        public SearchHospitalUsersQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.KeywordSearchType).InclusiveBetween(1, 3).WithMessage("검색 키워드 조회 타입이 범위를 벗어났습니다.");
        }
    }

    public class SearchHospitalUsersQueryHandler : IRequestHandler<SearchHospitalUsersQuery, Result<ListResult<SearchHospitalUsersResult>>>
    {
        private readonly ILogger<SearchHospitalUsersQueryHandler> _logger;
        private readonly IHospitalUserStore _hospitalUserStore;
        private readonly ICryptoService _cryptoService;

        public SearchHospitalUsersQueryHandler(
            IHospitalUserStore hospitalUserStore,
            ILogger<SearchHospitalUsersQueryHandler> logger,
            ICryptoService cryptoService)
        {
            _hospitalUserStore = hospitalUserStore;
            _logger = logger;
            _cryptoService = cryptoService;
        }

        public async Task<Result<ListResult<SearchHospitalUsersResult>>> Handle(SearchHospitalUsersQuery req, CancellationToken ct)
        {
            _logger.LogInformation("SearchHospitalUsersQueryHandler started.");

            var response = await _hospitalUserStore.SearchHospitalUsersAsync(
                req.PageNo, req.PageSize, req.FromDate, req.ToDate, req.KeywordSearchType, req.SearchKeyword, ct);

            foreach (var item in response.Items)
            {
                var name = _cryptoService.DecryptWithNoVector(item.Name, CryptoKeyType.Email);
                var phone = _cryptoService.DecryptWithNoVector(item.Phone, CryptoKeyType.Mobile);
                phone = phone.Replace("-", "");
                item.Name = name.Substring(0, 1).PadRight(name.Length, '*');
                item.Email = _cryptoService.DecryptWithNoVector(item.Email, CryptoKeyType.Email);
                item.Said = string.IsNullOrEmpty(item.AuthDt) ? 0 : 1;
                item.Phone = phone.Length == 11 ? phone.Substring(0, 3) + "-" + phone.Substring(3, 2) + "**" + "-" + phone.Substring(7, 2) + "**" : "";
                item.RegDtView = Convert.ToDateTime(item.RegDt).ToString("yyyy-MM-dd HH:mm");
                item.Email = item.Email == "null" ? "" : item.Email;
            }

            response.Items.OrderByDescending(x => x.RegDt).ToList();

            var startRowNum = response.TotalCount > 0 ? response.TotalCount - ((req.PageNo - 1) * req.PageSize) : 0;

            foreach (var item in response.Items)
            {
                item.RowNum = startRowNum--;
                item.LoginTypeName = GetLoginTYpeName(item.LoginType);
            }

            return Result.Success(response);
        }

        private string GetLoginTYpeName(string loginType)
        {
            var loginTypeName = string.Empty;

            switch (loginType)
            {
                case "E":
                    loginTypeName = "이메일";
                    break;
                case "K":
                    loginTypeName = "카카오톡";
                    break;
                case "F":
                    loginTypeName = "페이스북";
                    break;
                case "N":
                    loginTypeName = "네이버";
                    break;
                case "A":
                    loginTypeName = "애플로그인";
                    break;
                default:
                    loginTypeName = string.Empty;
                    break;
            }

            return loginTypeName;
        }
    }
}
