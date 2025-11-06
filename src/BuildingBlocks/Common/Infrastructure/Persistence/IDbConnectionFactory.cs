using System.Data;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
