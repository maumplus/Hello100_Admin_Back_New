namespace Hello100Admin.Modules.Admin.Application.Common.Models
{
    public sealed class ListResult<T>
    {
        public int TotalCount { get; init; }
        public IList<T> Items { get; init; } = [];
    }
}
