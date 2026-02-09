namespace Hello100Admin.Modules.Admin.Infrastructure.Configuration.Options
{
    public sealed class SftpOptions
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; } = 22;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string RemotePath { get; set; } = default!;
        public int ConnectTimeoutSeconds { get; set; } = 15;
        public int OperationTimeoutSeconds { get; set; } = 90;
        public int KeepAliveInterval { get; set; } = 30;
    }
}
