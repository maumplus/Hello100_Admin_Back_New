using System.Text.RegularExpressions;
using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Queries
{
    public record GetRequestBugsQuery(int PageNo, int PageSize, bool ApprYn) : IQuery<Result<ListResult<GetRequestBugsResult>>>;

    public class GetRequestBugsQueryValidator : AbstractValidator<GetRequestBugsQuery>
    {
        public GetRequestBugsQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
        }
    }

    public class GetRequestBugsQueryHandler : IRequestHandler<GetRequestBugsQuery, Result<ListResult<GetRequestBugsResult>>>
    {
        private readonly ILogger<GetRequestBugsQueryHandler> _logger;
        private readonly IRequestsManagementStore _requestsManagementStore;
        private readonly ICryptoService _cryptoService;

        public GetRequestBugsQueryHandler(
            IRequestsManagementStore requestsManagementStore,
            ILogger<GetRequestBugsQueryHandler> logger,
            ICryptoService cryptoService)
        {
            _requestsManagementStore = requestsManagementStore;
            _logger = logger;
            _cryptoService = cryptoService;
        }

        public async Task<Result<ListResult<GetRequestBugsResult>>> Handle(GetRequestBugsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetRequestBugsQuery started.");

            var requestBugsList = await _requestsManagementStore.GetRequestBugsAsync(req.PageSize, req.PageNo, req.ApprYn, ct);

            return Result.Success(requestBugsList);
        }
    }
}
