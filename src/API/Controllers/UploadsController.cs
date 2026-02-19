using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    [Auth]
    [Route("api/uploads")]
    public class UploadsController : BaseController
    {
        private readonly string _adminImageUrl;
        private readonly ILogger<UploadsController> _logger;
        private readonly ISftpClientService _sftpClientService;

        public UploadsController(IConfiguration config, ILogger<UploadsController> logger, ISftpClientService sftpClientService)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _logger = logger;
            _sftpClientService = sftpClientService;
        }

        public record FileUploadRequest(IFormFile File);

        /// <summary>
        /// 에디터 이미지 업로드 (단일)
        /// </summary>
        [HttpPost("editor-image")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FileUpload([FromForm] FileUploadRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/uploads/image [{AId}]", Aid);

            var payload = GetImagePayload(req.File);

            // 웹 리치 텍스트 에디터로 작성되는 HTML 콘텐츠 영역
            var path = await _sftpClientService.UploadImageWithPathAsync(payload, ImageUploadType.CK, "editor");

            string fullUrl = $"{_adminImageUrl}{path}";

            var result = Result.Success(fullUrl);

            return result.ToActionResult(this);
        }

        #region INTERNAL METHOD AREA **********************************************
        private FileUploadPayload? GetImagePayload(IFormFile? image)
        {
            if (image == null)
                return null;

            var payload = new FileUploadPayload(
                image.FileName,
                image.ContentType,
                image.Length,
                () => image.OpenReadStream()
            );

            return payload;
        }
        #endregion
    }
}
