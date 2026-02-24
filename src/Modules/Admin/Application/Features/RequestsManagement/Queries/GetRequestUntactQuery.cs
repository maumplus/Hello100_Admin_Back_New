using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Queries
{
    public record GetRequestUntactQuery(int Seq, string RootUrl): IQuery<Result<GetRequestUntactResult>>;

    public class GetRequestUntactQueryValidator : AbstractValidator<GetRequestUntactQuery>
    {
        public GetRequestUntactQueryValidator()
        {
            RuleFor(x => x.Seq).NotNull().WithMessage("요청ID는 필수입니다.");
        }
    }

    public class GetRequestUntactQueryHandler : IRequestHandler<GetRequestUntactQuery, Result<GetRequestUntactResult>>
    {
        private readonly ILogger<GetRequestUntactQueryHandler> _logger;
        private readonly IRequestsManagementStore _requestsManagementStore;
        private readonly ICryptoService _cryptoService;

        public GetRequestUntactQueryHandler(
            IRequestsManagementStore requestsManagementStore,
            ILogger<GetRequestUntactQueryHandler> logger,
            ICryptoService cryptoService)
        {
            _requestsManagementStore = requestsManagementStore;
            _logger = logger;
            _cryptoService = cryptoService;
        }

        public async Task<Result<GetRequestUntactResult>> Handle(GetRequestUntactQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetRequestUntactQuery");

            var requestUntact = await _requestsManagementStore.GetRequestUntactAsync(req.Seq, req.RootUrl, ct);

            return Result.Success(requestUntact);
        }
    }
}
