using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.Modules.Admin.Application.Features.Account.Commands;

namespace Hello100Admin.API.Controllers
{
    [Route("api/account")]
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 신규계정 병원 매핑(요양기관번호, 요양기관키)
        /// </summary>
        [HttpPost("set-hosp-no")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetHospNo(SetHospNoCommand command)
        {
            _logger.LogInformation("GET /api/account/set-hosp-no/ [{Aid}]", Aid);

            var result = await _mediator.Send(command);

            return result.ToActionResult(this);
        }
    }
}
