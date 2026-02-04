using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    [Auth]
    [Route("api/file-upload")]
    public class FileUploadController : BaseController
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IMediator _mediator;
        private readonly ISftpClientService _sftpClientService;

        public FileUploadController(IMediator mediator, ILogger<FileUploadController> logger, ISftpClientService sftpClientService)
        {
            _logger = logger;
            _mediator = mediator;
            _sftpClientService = sftpClientService;
        }

        /// <summary>
        /// 이미지 업로드 (테스트 용)
        /// </summary>
        [HttpPost("upload")]
        [AllowAnonymous]
        public async Task<IActionResult> FileUpload([FromForm] FileUploadRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/file-upload/upload [{AId}]", Aid);

            var payload = new FileUploadPayload(req.File.FileName, req.File.ContentType, req.File.Length, () => req.File.OpenReadStream());

            var path = await _sftpClientService.UploadImageWithPathAsync(payload, ImageUploadType.HO, "SuperAdmin");

            var result = Result.Success();

            return result.ToActionResult(this);
        }

        public record FileUploadRequest(IFormFile File);
    }
}
