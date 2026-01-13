using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Commands.AdminUser;

/// <summary>
/// 관리자 삭제 Command (Soft Delete)
/// </summary>
public record DeleteAdminUserCommand(string Uid) : ICommand<Result>;
