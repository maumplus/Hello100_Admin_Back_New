// using Hello100Admin.Services.Auth.Domain.Entities;
// using Microsoft.EntityFrameworkCore;

// namespace Hello100Admin.Services.Auth.Infrastructure.Persistence
// {
//     // 테스트 전용 DbContext: 실제 EF 마이그레이션/런타임은 인프라에서 제거되었으나
//     // Integration tests는 DbContext 타입을 필요로 하므로 테스트 프로젝트에 로컬로 제공합니다.
//     public class AuthDbContext : DbContext
//     {
//         public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
//         {
//         }

//         public DbSet<User> Users => Set<User>();
//         public DbSet<Role> Roles => Set<Role>();
//         public DbSet<Permission> Permissions => Set<Permission>();
//         public DbSet<UserRole> UserRoles => Set<UserRole>();
//         public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
//         public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
//     }
// }
