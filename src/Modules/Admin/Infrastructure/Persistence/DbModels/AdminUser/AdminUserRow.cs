namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.AdminUser;

internal sealed record AdminUserRow
{
    public required string Aid { get; init; }
    public string AccountId { get; init; } = default!;
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Role { get; init; }
    public string? Password { get; init; }
    public string? Salt { get; init; }
    public string? DelYn { get; init; }
    public bool IsDeleted { get; init; }
    public long RegDt { get; init; }
    public string? AccountLocked { get; init; }
    public string? Approved { get; init; }
    public string? Enabled { get; init; }
}
