using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage
{
    public interface IServiceUsageRepository
    {
        public Task<int> SubmitAlimtalkApplicationAsync(SubmitAlimtalkApplicationCommand req, CancellationToken cancellationToken = default);
    }
}
