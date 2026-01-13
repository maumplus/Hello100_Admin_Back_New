namespace Hello100Admin.API.Constracts.Admin.AdminUser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="UserId">사용자 ID</param>
    /// <param name="NewPassword">변경할 신규 비밀번호</param>
    public record UpdatePasswordRequest(string UserId, string NewPassword);
}
