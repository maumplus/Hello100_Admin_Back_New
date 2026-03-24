using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Hello100Admin.API.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ChartTypeAuthAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
    {
        public IReadOnlyCollection<ChartType> AllowedTypes { get; }

        public ChartTypeAuthAttribute(params ChartType[] types)
        {
            if (types is null || types.Length == 0)
                throw new ArgumentException("최소 1개 이상의 ChartType이 필요합니다.", nameof(types));

            AllowedTypes = types.Distinct().ToArray();
        }

        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}
