namespace Hello100Admin.Modules.Admin.Application.Common.Models
{
    public sealed record FileUploadPayload(
        string FileName,
        string ContentType,
        long Length,
        Func<Stream> OpenReadStream
    );
}
