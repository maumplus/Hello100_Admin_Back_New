using Microsoft.Extensions.Diagnostics.HealthChecks;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.HealthChecks
{
    public class MySqlDapperHealthCheck : IHealthCheck
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly string _testQuery;

        public MySqlDapperHealthCheck(IDbConnectionFactory connectionFactory, string testQuery = "SELECT 1")
        {
            _connectionFactory = connectionFactory;
            _testQuery = testQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = _testQuery;
                var result = command.ExecuteScalar();
                return HealthCheckResult.Healthy("MySQL connection OK");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"MySQL connection failed: {ex.Message}");
            }
        }
    }
}
