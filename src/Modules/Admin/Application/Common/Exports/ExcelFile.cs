namespace Hello100Admin.Modules.Admin.Application.Common.Exports
{
    public record ExcelFile(byte[] Content = default!, string FileName = default!, string ContentType = default!);
}
