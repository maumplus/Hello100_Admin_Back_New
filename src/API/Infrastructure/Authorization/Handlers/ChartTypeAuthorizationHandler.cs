using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Hello100Admin.API.Infrastructure.Authorization.Handlers
{
    public sealed class ChartTypeAuthorizationHandler
        : AuthorizationHandler<ChartTypeAuthAttribute>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ChartTypeAuthAttribute requirement)
        {
            var claimValue = context.User.FindFirst("chartType")?.Value;

            if (string.IsNullOrWhiteSpace(claimValue) == true)
                return Task.CompletedTask;

            if (Enum.TryParse<ChartType>(claimValue, ignoreCase: true, out var chartType) == false)
                return Task.CompletedTask;

            if (chartType == ChartType.All || requirement.AllowedTypes.Contains(chartType))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
