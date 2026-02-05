using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Models;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.External
{
    public interface ISftpClientService
    {
        /// <summary>
        /// 이미지 업로드 후 경로 반환
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadType"></param>
        /// <param name="customRootDirectory"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<string> UploadImageWithPathAsync(FileUploadPayload? file, ImageUploadType uploadType, string? customRootDirectory = null, CancellationToken ct = default);

        /// <summary>
        /// 여러 이미지 업로드 후 경로 리스트 반환
        /// </summary>
        /// <param name="files"></param>
        /// <param name="uploadType"></param>
        /// <param name="customRootDirectory"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<IReadOnlyList<string>> UploadImagesWithPathAsync(IReadOnlyList<FileUploadPayload>? files, ImageUploadType uploadType, string? customRootDirectory = null, CancellationToken ct = default);

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task DeleteFileAsync(string? fullPath, CancellationToken ct);
    }
}
