using System.Globalization;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Hello100Admin.Modules.Admin.Infrastructure.External.Sftp
{
    public class SftpClientService : ISftpClientService
    {
        #region FIELD AREA **********************************************
        private readonly string[] _imageAllowExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

        private readonly SftpOptions _opt;
        private readonly ILogger<SftpClientService> _logger;
        #endregion

        #region CONSTRUCTOR AREA ****************************************
        public SftpClientService(IOptions<SftpOptions> opt, ILogger<SftpClientService> logger)
        {
            _opt = opt.Value;
            _logger = logger;
        }
        #endregion

        #region INTERNAL CREATE SFTP CLIENT AREA *************************************
        private SftpClient CreateSftpClient()
        {
            var connectionInfo = new ConnectionInfo(
                _opt.Host,
                _opt.Port,
                _opt.Username,
                new PasswordAuthenticationMethod(_opt.Username, _opt.Password))
            {
                // SSH 핸드셰이크/키교환 시간
                Timeout = TimeSpan.FromSeconds(_opt.ConnectTimeoutSeconds)
            };

            var client = new SftpClient(connectionInfo)
            {
                // 파일 1개 업로드에 허용할 최대 시간
                OperationTimeout = TimeSpan.FromSeconds(_opt.OperationTimeoutSeconds)
            };

            client.KeepAliveInterval = TimeSpan.FromSeconds(_opt.KeepAliveInterval);

            return client;
        }

        #endregion

        #region ISFTPCLIENTSERVICE IMPLEMENTS AREA **********************************************
        public async Task<string> UploadImageWithPathAsync(FileUploadPayload? file, ImageUploadType uploadType, string? customRootDirectory = null, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (file is null || string.IsNullOrWhiteSpace(file.FileName))
                throw new BizException(AdminErrorCode.NotFoundFileName.ToError());

            if (file.Length <= 0)
                throw new BizException(AdminErrorCode.NotFoundFileStream.ToError());

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_imageAllowExtensions.Contains(ext))
                throw new BizException(AdminErrorCode.NotAllowedExtensions.ToError());

            var safeBaseName = SanitizeFileName(Path.GetFileNameWithoutExtension(file.FileName));
            var suffix = Guid.NewGuid().ToString("N")[..8];
            var newFileName = $"{safeBaseName}_{suffix}{ext}";

            var datePath = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var basePath = RemoteCombine(uploadType.ToString(), datePath);

            if (!string.IsNullOrWhiteSpace(customRootDirectory))
                basePath = RemoteCombine(customRootDirectory, basePath);

            var remoteDir = RemoteCombine(_opt.RemotePath, basePath);
            var remoteFilePath = RemoteCombine(remoteDir, newFileName);

            using var sftp = this.CreateSftpClient();

            try
            {
                await sftp.ConnectAsync(ct).ConfigureAwait(false);

                if (!sftp.IsConnected)
                    throw new BizException(AdminErrorCode.SftpConnectionFailed.ToError());

                await EnsureDirectoryAsync(sftp, remoteDir, ct).ConfigureAwait(false);

                using var input = file.OpenReadStream();

                if (uploadType == ImageUploadType.HO)
                {
                    // 애니메이션(GIF/WebP)이면 변환 금지 → 원본 업로드
                    if (ext is ".gif" or ".webp")
                    {
                        await sftp.UploadFileAsync(input, remoteFilePath, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        await using var hoStream = await TransformHoImageAsync(input, ext, ct).ConfigureAwait(false);
                        await sftp.UploadFileAsync(hoStream, remoteFilePath, ct).ConfigureAwait(false);
                    }
                }
                else
                {
                    // ImageUploadType.HO 외에는 원본 업로드
                    await sftp.UploadFileAsync(input, remoteFilePath, ct).ConfigureAwait(false);
                }

                // SFTP WorkingDirectory는 Upload 경로를 포함하고 있으나, URL의 경우 Upload를 포함하지 않으므로 경로 추가
                basePath = RemoteCombine("Upload", basePath); 

                return RemoteCombine(basePath, newFileName);
            }
            catch (BizException ex)
            {
                _logger.LogError(ex, "SFTP 업로드 실패: OriginalFileName={FileName}, RemotePath={RemoteFilePath}", file.FileName, remoteFilePath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SFTP 업로드 실패: OriginalFileName={FileName}, RemotePath={RemoteFilePath}", file.FileName, remoteFilePath);
                throw new BizException(AdminErrorCode.SftpUploadFailed.ToError());
            }
            finally
            {
                if (sftp.IsConnected) sftp.Disconnect();
            }
        }

        public async Task<IReadOnlyList<string>> UploadImagesWithPathAsync(IReadOnlyList<FileUploadPayload>? files, ImageUploadType uploadType, string? customRootDirectory = null, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (files is null || files.Count == 0)
                return Array.Empty<string>();

            var datePath = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var basePath = RemoteCombine(uploadType.ToString(), datePath);

            if (!string.IsNullOrWhiteSpace(customRootDirectory))
                basePath = RemoteCombine(customRootDirectory, basePath);

            var remoteDir = RemoteCombine(_opt.RemotePath, basePath);

            using var sftp = this.CreateSftpClient();

            try
            {
                await sftp.ConnectAsync(ct).ConfigureAwait(false);

                if (!sftp.IsConnected)
                    throw new BizException(AdminErrorCode.SftpConnectionFailed.ToError());

                await EnsureDirectoryAsync(sftp, remoteDir, ct).ConfigureAwait(false);

                var results = new List<string>(files.Count);

                foreach (var file in files)
                {
                    ct.ThrowIfCancellationRequested();

                    if (file is null || string.IsNullOrWhiteSpace(file.FileName))
                        throw new BizException(AdminErrorCode.NotFoundFileName.ToError());

                    if (file.Length <= 0)
                        throw new BizException(AdminErrorCode.NotFoundFileStream.ToError());

                    var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext) || !_imageAllowExtensions.Contains(ext))
                        throw new BizException(AdminErrorCode.NotAllowedExtensions.ToError());

                    var safeBaseName = SanitizeFileName(Path.GetFileNameWithoutExtension(file.FileName));
                    var suffix = Guid.NewGuid().ToString("N")[..8];
                    var newFileName = $"{safeBaseName}_{suffix}{ext}";

                    var remoteFilePath = RemoteCombine(remoteDir, newFileName);

                    using var input = file.OpenReadStream();

                    if (uploadType == ImageUploadType.HO)
                    {
                        // 애니메이션(GIF/WebP)이면 변환 금지 → 원본 업로드
                        if (ext is ".gif" or ".webp")
                        {
                            await sftp.UploadFileAsync(input, remoteFilePath, ct).ConfigureAwait(false);
                        }
                        else
                        {
                            await using var hoStream = await TransformHoImageAsync(input, ext, ct).ConfigureAwait(false);
                            await sftp.UploadFileAsync(hoStream, remoteFilePath, ct).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        // ImageUploadType.HO 외에는 원본 업로드
                        await sftp.UploadFileAsync(input, remoteFilePath, ct).ConfigureAwait(false);
                    }

                    var returnBasePath = RemoteCombine("Upload", basePath);
                    results.Add(RemoteCombine(returnBasePath, newFileName));
                }

                return results;
            }
            catch (BizException ex)
            {
                _logger.LogError(ex, "SFTP 멀티 업로드 실패: Count={Count}, UploadType={UploadType}, RemoteDir={RemoteDir}",
                    files.Count, uploadType, remoteDir);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SFTP 멀티 업로드 실패: Count={Count}, UploadType={UploadType}, RemoteDir={RemoteDir}",
                    files.Count, uploadType, remoteDir);
                throw new BizException(AdminErrorCode.SftpUploadFailed.ToError());
            }
            finally
            {
                if (sftp.IsConnected) sftp.Disconnect();
            }
        }


        public async Task DeleteFileAsync(string? fullPath, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fullPath)) return;

                using var sftp = this.CreateSftpClient();

                await sftp.ConnectAsync(ct);

                if (!sftp.IsConnected)
                    throw new BizException(AdminErrorCode.SftpConnectionFailed.ToError());

                //var working = sftp.WorkingDirectory; → /storage/Upload/efs_dev/efs/hello100/Upload
                sftp.ChangeDirectory("..");

                if (sftp.Exists(fullPath))
                    await sftp.DeleteFileAsync(fullPath, ct);
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, "파일 삭제 실패: {Path}", fullPath);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SFTP 파일 삭제 실패: {Path}", fullPath);
            }
        }
        #endregion

        #region INTERNAL METHOD AREA **********************************************
        private async Task<Stream> TransformHoImageAsync(Stream input, string ext, CancellationToken ct)
        {
            const int targetW = 1000;
            const int targetH = 500;

            var output = new MemoryStream(256 * 1024);

            using var image = await SixLabors.ImageSharp.Image.LoadAsync(input, ct).ConfigureAwait(false);

            image.Mutate(x => x.AutoOrient());

            var w = image.Width;
            var h = image.Height;

            image.Mutate(ctx =>
            {
                if (h >= targetH && w >= targetW)
                    ctx.Crop(new SixLabors.ImageSharp.Rectangle(0, 0, targetW, targetH));
                else if (h >= targetH)
                {
                    ctx.Crop(new SixLabors.ImageSharp.Rectangle(0, 0, w, targetH));
                    ctx.Resize(targetW, targetH);
                }
                else if (w >= targetW)
                {
                    ctx.Crop(new SixLabors.ImageSharp.Rectangle(0, 0, targetW, h));
                    ctx.Resize(targetW, targetH);
                }
                else
                {
                    ctx.Resize(targetW, targetH);
                }
            });

            await SaveByExtensionAsync(image, output, ext, ct).ConfigureAwait(false);

            output.Position = 0;
            return output;
        }

        private async Task SaveByExtensionAsync(
            SixLabors.ImageSharp.Image image,
            Stream output,
            string ext,
            CancellationToken ct)
        {
            ext = ext.ToLowerInvariant();

            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    await image.SaveAsync(output, new JpegEncoder { Quality = 80 }, ct).ConfigureAwait(false);
                    break;

                case ".png":
                    await image.SaveAsync(output, new PngEncoder(), ct).ConfigureAwait(false);
                    break;

                case ".gif":
                    await image.SaveAsync(output, new GifEncoder(), ct).ConfigureAwait(false);
                    break;

                case ".bmp":
                    await image.SaveAsync(output, new BmpEncoder(), ct).ConfigureAwait(false);
                    break;

                case ".webp":
                    await image.SaveAsync(output, new WebpEncoder(), ct).ConfigureAwait(false);
                    break;

                default:
                    throw new NotSupportedException($"지원하지 않는 이미지 확장자: {ext}");
            }
        }

        private async Task EnsureDirectoryAsync(SftpClient sftp, string remoteDir, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var normalized = remoteDir.Replace("\\", "/");
            var isAbs = normalized.StartsWith("/", StringComparison.Ordinal);
            var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);

            var current = isAbs ? "/" : "";

            foreach (var part in parts)
            {
                current = current == "/" ? "/" + part
                        : string.IsNullOrEmpty(current) ? part
                        : current + "/" + part;

                try
                {
                    await sftp.CreateDirectoryAsync(current, ct).ConfigureAwait(false);
                }
                catch(Exception e)
                {
                    if (sftp.Exists(current) == false)
                    {
                        _logger.LogError(e, "SFTP 디렉토리 생성 실패: {Dir}", current);
                        throw;
                    }
                }
            }
        }

        private static string RemoteCombine(params string[] parts)
        {
            if (parts is null || parts.Length == 0) return string.Empty;

            var first = parts.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p)) ?? string.Empty;
            var isAbs = first.TrimStart().StartsWith("/", StringComparison.Ordinal);

            var tokens = parts.Where(p => !string.IsNullOrWhiteSpace(p))
                              .Select(p => p.Replace("\\", "/"))
                              .SelectMany(p => p.Split('/', StringSplitOptions.RemoveEmptyEntries))
                              .ToArray();

            var combined = string.Join("/", tokens);
            return isAbs ? "/" + combined : combined;
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "file";

            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name.Trim();
        }
        #endregion
    }
}
