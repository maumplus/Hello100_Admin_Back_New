namespace Hello100Admin.Modules.Admin.Infrastructure.Models;

public class AdminUserDbModel
{
    public required string Aid { get; set; }
    public string AccountId { get; set; } = default!;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public string? Password { get; set; }
    public string? Salt { get; set; }
    public string? DelYn { get; set; }
    public bool IsDeleted { get; set; }
    public long RegDt { get; set; }
    public string? AccountLocked { get; set; }
    public string? Approved { get; set; }
    public string? Enabled { get; set; }
}
