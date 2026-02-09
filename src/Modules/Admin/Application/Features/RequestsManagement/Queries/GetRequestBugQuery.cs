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
    public record GetRequestBugQuery: IQuery<Result<GetRequestBugResult>>
    {
        public long HpId { get; init; }
    }

    public class GetRequestBugQueryValidator : AbstractValidator<GetRequestBugQuery>
    {
        public GetRequestBugQueryValidator()
        {
            RuleFor(x => x.HpId).NotNull().WithMessage("요청ID는 필수입니다.");
        }
    }

    public class GetRequestBugQueryHandler : IRequestHandler<GetRequestBugQuery, Result<GetRequestBugResult>>
    {
        private readonly ILogger<GetRequestBugQueryHandler> _logger;
        private readonly IRequestsManagementStore _requestsManagementStore;
        private readonly ICryptoService _cryptoService;

        public GetRequestBugQueryHandler(
            IRequestsManagementStore requestsManagementStore,
            ILogger<GetRequestBugQueryHandler> logger,
            ICryptoService cryptoService)
        {
            _requestsManagementStore = requestsManagementStore;
            _logger = logger;
            _cryptoService = cryptoService;
        }

        public async Task<Result<GetRequestBugResult>> Handle(GetRequestBugQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetRequestBugQuery");

            var requestBug = await _requestsManagementStore.GetRequestBugAsync(req.HpId, ct);

            return Result.Success(requestBug);
        }
    }
}
